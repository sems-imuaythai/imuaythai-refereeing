using Imuaythai.Refereeing.Models;
using Imuaythai.Refereeing.Services;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Imuaythai.Refereeing.Hubs
{
    public class RingHub : Hub
    {
        private readonly IFightService _fightService;

        public RingHub(IFightService fightService)
        {
            _fightService = fightService;
            _fightService.OnBreakIsOver += OnBreakIsOver;
            _fightService.OnRoundIsOver += OnRoundIsOver;
        }

        private async void OnRoundIsOver(int fightId)
        {
            await Clients.All.SendAsync(ClientMethods.RoundEnded, fightId);
        }

        private async void OnBreakIsOver(int fightId)
        {
            await Clients.All.SendAsync(ClientMethods.BreakEnded, fightId);
        }

        [HubMethodName("Join")]
        public async Task Join(string userId, string role)
        {
            var addToRoleGroup = Groups.AddToGroupAsync(Context.ConnectionId, role);
            var addToSingleGroup = Groups.AddToGroupAsync(Context.ConnectionId, userId);
            await Task.WhenAll(addToRoleGroup, addToSingleGroup);
        }

        [HubMethodName("GetFights")]
        public async Task GetFightList(char ring, int constestId)
        {
            var fights = await _fightService.GetFightListAsync(ring, constestId);
            await Clients.Caller.SendAsync(ClientMethods.FightListReceived, fights);
        }

        [HubMethodName("EnterToFight")]
        public async Task GetFight(int fightId)
        {
            var fight = await _fightService.GetFightAsync(fightId);
            await Clients.Caller.SendAsync(ClientMethods.EnteredToFight, fight);
        }

        [HubMethodName("ResumeFight")]
        public async Task ResumeFight(int fightId)
        {
            var fightTime = await _fightService.ResumeFightAsync(fightId);
            await Clients.All.SendAsync(ClientMethods.FightResumed, fightTime);
        }

        [HubMethodName("PauseFight")]
        public async Task PauseFight(int fightId)
        {
            var fightTime = await _fightService.PauseFightAsync(fightId);
            await Clients.All.SendAsync(ClientMethods.FightPaused, fightTime);
        }

        [HubMethodName("EndFight")]
        public async Task EndFight(int fightId)
        {
            await _fightService.EndFightAsync(fightId);
            await Clients.All.SendAsync(ClientMethods.FightEnded);
        }

        [HubMethodName("SavePoints")]
        public async Task SavePoints(int fightId, int roundId, JudgePoints points)
        {
            var savedPoints = await _fightService.SavePointsAsync(fightId, roundId, points);
            var sendToMainJudge = Clients.Group(ClientGroups.MainJudge).SendAsync(ClientMethods.JudgeSentPoints);
            var sentToJudge = Clients.Caller.SendAsync(ClientMethods.PointsSaved);
            await Task.WhenAll(sendToMainJudge, sentToJudge);
        }

        [HubMethodName("AcceptPoints")]
        public async Task AcceptPoints(int fightId, Guid judgePointsId)
        {
            var points = await _fightService.AcceptPointsAsync(fightId, judgePointsId);
            var clientGroup = judgePointsId.ToString();
            await Clients.Group(clientGroup).SendAsync(ClientMethods.PointsAccepted, points);
        }

        [HubMethodName("PrematureEnd")]
        public async Task PrematureEnd()
        {
            await Clients.All.SendAsync(ClientMethods.PrematureEnded);
        }

        [HubMethodName("SaveInjury")]
        public async Task SavePremature(Injury injury)
        {
            await _fightService.SaveInjuryAsync(injury);
            await Clients.Caller.SendAsync(ClientMethods.InjurySaved);
        }

        [HubMethodName("StartRound")]
        public async Task StartRound(int fightId)
        {
            var points = await _fightService.StartRoundAsync(fightId);

            var sendPoints = points
                .GroupBy(p => p.JudgeId)
                .Select(p => new { JudgeId = p.Key.ToString(), Points = p.ToArray()})
                .Select(p => Clients.Group(p.JudgeId).SendAsync(ClientMethods.RoundStarted, p.Points));

            await Task.WhenAll(sendPoints);
            await Clients.Group(ClientGroups.TimeKeeper).SendAsync(ClientMethods.RoundStarted);
        }

        [HubMethodName("EndRound")]
        public async Task EndRound(int fightId)
        {
            await _fightService.EndRoundAsync(fightId);
            await Clients.All.SendAsync(ClientMethods.RoundEnded);
        }
    }
}
