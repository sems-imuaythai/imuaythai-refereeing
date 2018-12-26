using System;

namespace Imuaythai.Refereeing.Models
{
    public class JudgePoints
    {
        public Guid Id { get; set; }
        public Guid FighterId { get; set; }
        public Guid JudgeId { get; set; }
        public int Points { get; set; }
        public int Cautions { get; set; }
        public int Warnings { get; set; }
        public int KnockDown { get; set; }
        public int J { get; set; }
        public int X { get; set; }
        public string Injury { get; set; }
        public int? InjuryTime { get; set; }
        public bool Accepted { get; set; }
    }
}