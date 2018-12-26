using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Imuaythai.Refereeing.Models;

namespace Imuaythai.Refereeing.Services
{
    public interface IFightService
    {
        Task<IEnumerable<PlainFight>> GetFightListAsync(char ring);
        Task<Fight> GetFightAsync(int fightId);
        Task<FightTime> ResumeFightAsync(int fightId);
        Task<FightTime> PauseFightAsync(int fightId);
        Task EndFightAsync(int fightId);
        Task<JudgePoints> SavePointsAsync(int fightId, int roundId, JudgePoints points);
        Task<JudgePoints> AcceptPointsAsync(int fightId, Guid judgePointsId);
        Task<JudgePoints> GetPointsAsync(int fightId, Guid judgePointsId);
        Task SaveInjuryAsync(Injury premature);
        Task<IEnumerable<JudgePoints>> StartRoundAsync(int fightId);
        Task EndRoundAsync(int fightId);

        event TimeIsOverHandler OnRoundIsOver;
        event TimeIsOverHandler OnBreakIsOver;
    }
}
