using System;

namespace Imuaythai.Refereeing.Models
{
    public class Injury
    {
        public int FightId { get; internal set; }
        public Guid JudgeId { get; internal set; }
        public Guid FighterId { get; internal set; }
        public string Description { get; internal set; }
    }
}