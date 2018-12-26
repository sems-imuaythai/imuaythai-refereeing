using System.Collections.Generic;

namespace Imuaythai.Refereeing.Services
{
    public interface IRoundTimer
    {
        int GetTime(int id);
        void Resume(int fightId);
        void Stop(int fightId);
        void Start(int fightId, int duration);
        void Pause(int fightId);

        event TimeIsOverHandler OnTimeIsOver;
    }

    public class RoundTimer : IRoundTimer
    {
        private readonly Dictionary<int, DurationTimer> _timers;

        public event TimeIsOverHandler OnTimeIsOver;

        public RoundTimer()
        {
            _timers = new Dictionary<int, DurationTimer>();
        }

        public int GetTime(int fightId)
        {
            var timer = _timers[fightId];
            return timer.EllapsedSeconds;
        }

        public void Pause(int fightId)
        {
            var timer = _timers[fightId];
            timer.Stop();
        }

        public void Resume(int fightId)
        {
            var timer = _timers[fightId];
            timer.Start();
        }

        public void Start(int fightId, int duration)
        {
            if (!_timers.ContainsKey(fightId))
            {
                var newTimer = new DurationTimer(fightId, duration);
                newTimer.OnTimeIsOver += OnTimeIsOver;
                _timers.Add(fightId, newTimer);
            }

            var timer = _timers[fightId];
            timer.Start();
        }

        public void Stop(int fightId)
        {
            var timer = _timers[fightId];
            timer.Reset();
            timer.Dispose();
            _timers.Remove(fightId);
        }
    }
}