using System;

namespace MultithreadCalculator
{

    public class StopwatchTimer
    {
        private DateTime? _start;
        private DateTime? _stop;
        private bool _running = false;

        public void Start()
        {
            _start = DateTime.UtcNow;
            _stop = null;
            _running = true;
        }

        public void Stop()
        {
            if (_running)
            {
                _stop = DateTime.UtcNow;
                _running = false;
            }
        }

        public void Reset()
        {
            _start = null;
            _stop = null;
            _running = false;
        }

 
        public TimeSpan Elapsed
        {
            get
            {
                if (_start == null) return TimeSpan.Zero;
                if (_running) return DateTime.UtcNow - _start.Value;
                if (_stop != null) return _stop.Value - _start.Value;
                return TimeSpan.Zero;
            }
        }

        public long ElapsedMilliseconds => (long)Elapsed.TotalMilliseconds;

        public override string ToString()
        {
            return Elapsed.ToString();
        }
    }
}
