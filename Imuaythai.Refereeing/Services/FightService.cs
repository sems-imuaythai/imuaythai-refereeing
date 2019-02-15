using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Imuaythai.Refereeing.Models;

namespace Imuaythai.Refereeing.Services
{
    public class FightService : IFightService
    {
        private readonly IFightStorage _fightStorage;
        private readonly IWinnerCalculator _winnerCalculator;
        private readonly IRoundTimer _roundTimer;
        private readonly IBreaksTimer _breaksTimer;

        public event TimeIsOverHandler OnRoundIsOver;
        public event TimeIsOverHandler OnBreakIsOver;

        public FightService(IFightStorage fightStorage, IWinnerCalculator winnerCalculator, IRoundTimer roundTimer, IBreaksTimer breaksTimer)
        {
            _fightStorage = fightStorage;
            _winnerCalculator = winnerCalculator;
            _roundTimer = roundTimer;
            _breaksTimer = breaksTimer;

            _breaksTimer.OnTimeIsOver += BreakIsOver;
            _roundTimer.OnTimeIsOver += RoundIsOver;
        }

        private void RoundIsOver(int fightId)
        {
            OnRoundIsOver?.Invoke(fightId);
        }

        private void BreakIsOver(int fightId)
        {
            OnBreakIsOver?.Invoke(fightId);
        }

        public async Task<JudgePoints> AcceptPointsAsync(int fightId, Guid judgePointsId)
        {
            var fight = await _fightStorage.GetAsync(fightId);
            var points = fight.GetPoints(judgePointsId);

            points.Accepted = true;

            await _fightStorage.SaveAsync(fight);
            return points;
        }

        public Task<Fight> GetFightAsync(int fightId)
        {
            return _fightStorage.GetAsync(fightId);
        }

        public Task<IEnumerable<PlainFight>> GetFightListAsync(char ring, int contestId)
        {
            return _fightStorage.GetAllAsync(ring, contestId);
        }

        public async Task<JudgePoints> GetPointsAsync(int fightId, Guid judgePointsId)
        {
            var fight = await _fightStorage.GetAsync(fightId);
            return fight.GetPoints(judgePointsId);
        }

        public async Task<JudgePoints> SavePointsAsync(int fightId, int roundNumber, JudgePoints points)
        {
            var fight = await _fightStorage.GetAsync(fightId);

            points.Id = points.Id == Guid.Empty ? Guid.NewGuid() : points.Id;

            var roundPoints = fight.Points.First(p => p.RoundNumber == roundNumber);
            roundPoints.Points.Add(points);

            await _fightStorage.SaveAsync(fight);

            return points;
        }

        public async Task SaveInjuryAsync(Injury injury)
        {
            var fight = await _fightStorage.GetAsync(injury.FightId);
            var roundPoints = fight.Points.First(p => p.RoundNumber == fight.CurrentRound);
            var judgePoints = roundPoints.Points.First(p => p.JudgeId == injury.JudgeId && p.FighterId == injury.FighterId);
            judgePoints.Injury = injury.Description;
            judgePoints.InjuryTime = _roundTimer.GetTime(injury.FightId);
            await _fightStorage.SaveAsync(fight);
        }

        public async Task<FightTime> PauseFightAsync(int fightId)
        {
            var fight = await _fightStorage.GetAsync(fightId);

            fight.State = FightState.Paused;
            await _fightStorage.SaveAsync(fight);

            var time = _roundTimer.GetTime(fightId);
            _roundTimer.Pause(fightId);

            return new FightTime
            {
                Time = time
            };
        }

        public async Task<FightTime> ResumeFightAsync(int fightId)
        {
            var fight = await _fightStorage.GetAsync(fightId);

            fight.State = FightState.InProgress;
            await _fightStorage.SaveAsync(fight);

            var time = _roundTimer.GetTime(fightId);
            _roundTimer.Resume(fightId);

            return new FightTime
            {
                Time = time
            };
        }

        public async Task EndFightAsync(int fightId)
        {
            var fight = await _fightStorage.GetAsync(fightId);
            fight.Winner = _winnerCalculator.GetWinner(fight.Points);
            fight.State = FightState.Ended;
            await _fightStorage.SaveAsync(fight);

            if (!fight.NextFightId.HasValue)
            {
                return;
            }

            var nextFight = await _fightStorage.GetAsync(fight.NextFightId.Value);
            if (nextFight.RedFighter == null)
            {
                nextFight.RedFighter = fight.Winner;
            }

            if (nextFight.BlueFighter == null)
            {
                nextFight.BlueFighter = fight.Winner;
            }

            await _fightStorage.SaveAsync(nextFight);
        }

        public async Task<IEnumerable<JudgePoints>> StartRoundAsync(int fightId)
        {
            var fight = await _fightStorage.GetAsync(fightId);
            fight.CurrentRound = fight.CurrentRound + 1;
            var judgesPoints = fight.Judges.SelectMany(judge => new[] { new JudgePoints
                {
                    Id = Guid.NewGuid(),
                    JudgeId = judge.Id,
                    FighterId = fight.RedFighter.Id
                },
                new JudgePoints{
                    Id = Guid.NewGuid(),
                    JudgeId = judge.Id,
                    FighterId = fight.BlueFighter.Id
                }
            }).ToList();

            fight.Points.Add(new RoundPoints
            {
                RoundNumber = fight.CurrentRound,
                Points = judgesPoints
            });

            await _fightStorage.SaveAsync(fight);

            _roundTimer.Start(fightId, fight.RoundsConfiguration.Duration);
            _breaksTimer.Stop(fightId);

            return judgesPoints;
        }

        public async Task EndRoundAsync(int fightId)
        {
            var fight = await _fightStorage.GetAsync(fightId);
            _roundTimer.Stop(fightId);
            _breaksTimer.Start(fightId, fight.RoundsConfiguration.BreakDuration);
        }
    }
}
