using System.Timers;

namespace Imuaythai.Refereeing.Services
{
    public class DurationTimer : Timer
    {
        public readonly int FightId;
        public readonly int Duration;

        public int EllapsedSeconds { get; private set; } = 0;
        public event TimeIsOverHandler OnTimeIsOver;

        public DurationTimer(int fightId, int duration)
        {
            FightId = fightId;
            Duration = duration;
            Interval = 1000;
            Elapsed += (s, e) =>
            {
                EllapsedSeconds++;

                if (EllapsedSeconds > Duration)
                {
                    Stop();
                    OnTimeIsOver?.Invoke(fightId);
                }
            };
        }

        public void Reset()
        {
            Stop();
            EllapsedSeconds = 0;
        }
    }

    public delegate void TimeIsOverHandler(int fightId);
}