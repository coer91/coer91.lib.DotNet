namespace coer91
{
    public class Scheduler
    {
        private readonly int _hour;
        private readonly int _minute;
        private readonly Exec _callBack;
        private System.Timers.Timer _scheduler;
        public delegate void Exec(object sender, System.Timers.ElapsedEventArgs args);


        public Scheduler(Exec callBack, int hour = 0, int minute = 0)
        {
            _hour = hour;
            _minute = minute;
            _callBack = callBack;

            if (hour < 0 || hour > 23)
                throw new ArgumentOutOfRangeException($"Hour {hour} Out Of Range");

            if (minute < 0 || minute > 59)
                throw new ArgumentOutOfRangeException($"Minute {minute} Out Of Range");

            Reset();
        }


        public void Reset()
        {
            Clear();

            DateTime schedulerDate = DateTime.Today.AddHours(_hour).AddMinutes(_minute);
            DateTime currentDate = DateTime.Now;

            if (currentDate >= schedulerDate)
                schedulerDate = schedulerDate.AddDays(1);

            _scheduler = new System.Timers.Timer();
            _scheduler.Interval = (schedulerDate - currentDate).TotalMilliseconds;
            _scheduler.Elapsed += new System.Timers.ElapsedEventHandler(_callBack);
            _scheduler.AutoReset = true;
            _scheduler.Enabled = false;
        }


        public void Stop()
        {
            _scheduler.Enabled = false;
            _scheduler.Stop();
        }


        public void Start()
        {
            _scheduler.Enabled = true;
            _scheduler.Start();
        }


        public void SetTime(int days = 0, int hours = 0, int minutes = 0, int seconds = 0)
        {
            Stop();
            _scheduler.Interval = 1;
            _scheduler.Interval += days * 86399999;
            _scheduler.Interval += hours * 3599999;
            _scheduler.Interval += minutes * 59999;
            _scheduler.Interval += seconds * 999;
        }


        public void Clear()
        {
            if (_scheduler is not null)
            {
                Stop();
                _scheduler.Close();
                _scheduler.Dispose();
                _scheduler = null;
            }
        }
    }
} 