using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bishop.Service
{
    /// <summary>
    /// A timer that instead of running tasks by an interval, keeps a certain gap between tasks.
    ///  (From end of execution to the start of next)
    /// </summary>
    public class GapBasedTimer : ITimer
    {
        private readonly TimeSpan NEVER = TimeSpan.FromMilliseconds(-1);
        private readonly ILogger _logger;
        private Timer timer = null;
        TimeSpan TimerSleepSpan;
        Func<Task> timerCallbackAsync;
        private bool disposedValue;

        public GapBasedTimer(Func<Task> callback = null, TimeSpan? sleepSpan = null, ILogger logger = null)
        {
            _logger = logger;
            var initialSleepSpan = sleepSpan ?? NEVER;
            ResetTimerProperties(callback, initialSleepSpan);
        }

        private void ResetTimerProperties(Func<Task> callback, TimeSpan sleepSpan)
        {
            _logger?.LogInformation("GapBasedTimer - ResetTimerProperties - started");
            if (callback != null)
            {
                this.timerCallbackAsync = callback;
            }

            this.TimerSleepSpan = sleepSpan;
        }

        public void InitializeCallback(Func<Task> callback, TimeSpan sleepSpan)
        {
            _logger?.LogInformation("GapBasedTimer - InitializeCallback - calling action");

            ResetTimerProperties(callback, sleepSpan);

            async Task timerCallBackWrapperAsync(object state)
            {
                ScheduleTimerToRun(NEVER);
                _logger?.LogInformation("GapBasedTimer - timerCallBackWrapper - calling action");
                try
                {
                    await timerCallbackAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"GapBasedTimer - InitializeCallback - timerCallbackAsync tried to raise callback and throw an exception: { ex }");
                    throw;
                }
                ScheduleTimerToRun(this.TimerSleepSpan);
            }

            _logger?.LogInformation("GapBasedTimer - InitializeCallback - Running first time");

            // Set Timer
            timer?.Dispose();
            _logger?.LogInformation("GapBasedTimer - InitializeCallback - wrapping action with timer");
            timer = new Timer(async (state) => await timerCallBackWrapperAsync(state));

            _logger?.LogInformation("GapBasedTimer - InitializeCallbackAsync - Scheduling timer for next run");
            ScheduleTimerToRun(this.TimerSleepSpan);
        }

        public void Pause()
        {
            _logger?.LogInformation("GapBasedTimer - Pause");
            ScheduleTimerToRun(NEVER);
        }

        public void Reset(TimeSpan? sleepSpan = null)
        {
            _logger?.LogInformation("GapBasedTimer - Reset");

            if (sleepSpan.HasValue)
            {
                this.TimerSleepSpan = sleepSpan.Value;
            }

            ScheduleTimerToRun(this.TimerSleepSpan);
        }

        void ScheduleTimerToRun(TimeSpan timerSleepSpanForNextRun)
        {
            _logger?.LogInformation($"GapBasedTimer - ScheduleTimerToRun time for next run: {timerSleepSpanForNextRun}");
            timer.Change(timerSleepSpanForNextRun, NEVER);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    timer?.Dispose();
                }

                timer = null;
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
