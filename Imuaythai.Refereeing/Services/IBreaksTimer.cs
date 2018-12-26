using System.Collections.Generic;
using System.Timers;

namespace Imuaythai.Refereeing.Services
{
    public interface IBreaksTimer
    {
        void Start(int fightId, int breakDuration);
        void Stop(int fightId);
        event TimeIsOverHandler OnTimeIsOver;
    }

    public class BreaksTimer : IBreaksTimer
    {
        private readonly Dictionary<int, DurationTimer> _timers;
        public event TimeIsOverHandler OnTimeIsOver;

        public BreaksTimer()
        {
            _timers = new Dictionary<int, DurationTimer>();
        }

        public void Start(int fightId, int breakDuration)
        {
            if (!_timers.ContainsKey(fightId))
            {
                var newTimer = new DurationTimer(fightId, breakDuration);
                newTimer.OnTimeIsOver += OnTimeIsOver;
                _timers.Add(fightId, newTimer);
            }

            var timer = _timers[fightId];
            timer.Start();
        }

        public void Stop(int fightId)
        {
            var timer = _timers[fightId];
            timer.Start();

            _timers.Remove(fightId);
            timer.Dispose();
        }
    }
}