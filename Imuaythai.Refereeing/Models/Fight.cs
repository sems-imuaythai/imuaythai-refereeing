using System;
using System.Collections.Generic;

namespace Imuaythai.Refereeing.Models
{
    public class Fight
    {
        public int Id { get; set; }

        public int? NextFightId { get; set; }

        public Fighter BlueFighter { get; set; }

        public Fighter RedFighter { get; set; }

        public Judge MainJudge { get; set; }

        public Judge Referee { get; set; }

        public Judge TimeKeeper { get; set; }

        public int ContestId { get; set; }

        public Fighter Winner { get; set; }

        public DateTime StartAt { get; set; }

        public char Ring { get; set; }

        public KnockOut KnockOut { get; set; }

        public string CategoryName { get; set; }

        public string WeightAge { get; set; }

        public RoundsConfiguration RoundsConfiguration { get; set; }

        public List<RoundPoints> Points { get; set; }

        public List<Judge> Judges { get; internal set; }

        public int CurrentRound { get; set; }

        public FightState State { get; set; }
    }

    public enum FightState { NotStarted, Paused, InProgress, Ended}
}
