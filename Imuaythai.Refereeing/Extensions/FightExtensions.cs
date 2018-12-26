using System;
using System.Linq;
using Imuaythai.Refereeing.Models;

namespace Imuaythai.Refereeing.Services
{
    public static class FightExtensions
    {
        public static JudgePoints GetPoints(this Fight fight, Guid pointsId)
        {
            return fight.Points
                .SelectMany(p => p.Points)
                .FirstOrDefault(p => p.Id == pointsId);
        }
    }
}
