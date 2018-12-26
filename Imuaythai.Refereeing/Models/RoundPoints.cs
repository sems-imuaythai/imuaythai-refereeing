using System.Collections.Generic;

namespace Imuaythai.Refereeing.Models
{
    public class RoundPoints
    {
        public int RoundNumber { get; set; }
        public List<JudgePoints> Points { get; set; }
    }
}