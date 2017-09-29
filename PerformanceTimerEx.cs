namespace RB3DOverlayed
{
    using System;
    using System.Diagnostics;
    using Clio.Utilities;
    using ff14bot.Helpers;
    
    /// <summary>A performance timer.</summary>
    public class PerformanceTimerEx : IDisposable
    {
        private readonly Stopwatch _timer;
        private readonly string _debugText;
        private bool _dontPrint;
        private readonly int _logmsLimit;

        /// <summary>Initializes a new instance of the <see cref="PerformanceTimer"/> class.</summary>
        /// <remarks>Created 2012-04-03.</remarks>
        /// <param name="debugText"> The debug text.</param>
        /// <param name="logMsLimit">(Optional) the log milliseconds limit.</param>
        public PerformanceTimerEx(string debugText, int logMsLimit = 0)
        {
            _timer = new Stopwatch();
            _debugText = debugText;
            _logmsLimit = logMsLimit;

            AutoStart();
        }

        /// <summary>Starts the timer.</summary>
        /// <remarks>Created 2012-04-03.</remarks>
        public void Start()
        {
            _timer.Start();
        }

        private void AutoStart()
        {
            Start();
        }

        private void AutoStop()
        {
            StopAndPrint();
        }

        /// <summary>Dont print.</summary>
        public void DontPrint()
        {
            _dontPrint = true;
        }

        /// <summary>Gets the elapsed milliseconds.</summary>
        /// <value>The elapsed milliseconds.</value>
        public long ElapsedMilliseconds { get { return _timer.ElapsedMilliseconds; } }

        /// <summary>Stops the timers and prints the time.</summary>
        /// <remarks>Created 2012-04-03.</remarks>
        public void StopAndPrint()
        {
            _timer.Stop();
            if (_dontPrint)
                return;

            if (ElapsedMilliseconds > _logmsLimit)
                Logging.WriteDiagnostic("[{0}ms] {1}", _timer.Elapsed.TotalMilliseconds, _debugText);
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.</summary>
        public void Dispose()
        {
            AutoStop();
        }
    }
}
