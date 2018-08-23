using Raven.Utils;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Atomics;

namespace Raven.Core {
    public class UpdateDeltaLoop {
        //vars
        private readonly Action func = null;
        private readonly Action<double> deltaFunc = null;

        private double targetFramerate = 0.0d;
        private long targetTicks = 0L;
        private long driftTargetTicks = 0L;
        private volatile int spinIterations = 1;

        private AtomicBoolean running = new AtomicBoolean(false);

        //constructor
        public UpdateDeltaLoop(Action func, double targetFramerate, int spinIterations = 10) {
            if (func == null) {
                throw new ArgumentNullException("func");
            }

            this.func = func;
            this.targetFramerate = MathUtil.Clamp(0.0d, double.MaxValue, targetFramerate);
            this.targetTicks = (targetFramerate == 0.0d) ? 0L : (long) Math.Floor((1000.0d / targetFramerate) * TimeSpan.TicksPerMillisecond);
            driftTargetTicks = targetTicks;
            this.spinIterations = spinIterations;
        }
        public UpdateDeltaLoop(Action<double> deltaFunc, double targetFramerate, int spinIterations = 10) {
            if (deltaFunc == null) {
                throw new ArgumentNullException("deltaFunc");
            }

            this.deltaFunc = deltaFunc;
            this.targetFramerate = MathUtil.Clamp(0.0d, double.MaxValue, targetFramerate);
            this.targetTicks = (targetFramerate == 0.0d) ? 0L : (long) Math.Floor((1000.0d / targetFramerate) * TimeSpan.TicksPerMillisecond);
            driftTargetTicks = targetTicks;
            this.spinIterations = spinIterations;
        }

        //public
        public double TargetFramerate {
            get {
                return this.targetFramerate;
            }
            set {
                if (value < 0.0d || double.IsNaN(value) || double.IsInfinity(value)) {
                    return;
                }

                this.targetFramerate = value;
                this.targetTicks = (targetFramerate == 0.0d) ? 0L : (long) Math.Floor((1000.0d / targetFramerate) * TimeSpan.TicksPerMillisecond);
                driftTargetTicks = targetTicks;
            }
        }
        public int SpinIterations {
            get {
                return spinIterations;
            }
            set {
                spinIterations = MathUtil.Clamp(1, int.MaxValue, value);
            }
        }

        public void Start() {
            if (running.CompareExchange(true, false)) {
                return;
            }

            Stopwatch watch = new Stopwatch();
            watch.Start();
            long lastFrameTime = 0L; // For deltaTime
            long drift = 0L; // Accounting for short-term spikes
            while (running) {
                lastFrameTime = Update(watch, lastFrameTime, ref drift);
            }
            watch.Stop();
        }
        public void Stop() {
            running.Value = false;
        }

        //private
        private long Update(Stopwatch watch, long lastFrameTime, ref long drift) {
            if (targetFramerate == 0.0d) {
                return targetTicks; // Faking perfect accuracy
            }

            long start = watch.Elapsed.Ticks; // High performance + Accuracy
            func?.Invoke();
            deltaFunc?.Invoke(lastFrameTime / (double) targetTicks);
            while (watch.Elapsed.Ticks - start < driftTargetTicks - drift) {
                Thread.SpinWait(spinIterations);
            }
            long end = watch.Elapsed.Ticks;

            // Preventing overflows
            drift = (long.MaxValue - end - start - targetTicks < drift) ? long.MaxValue : drift + end - start - targetTicks;
            // Accounting for long-term lag
            if (drift > 0) {
                driftTargetTicks--;
            } else if (drift < 0) {
                driftTargetTicks++;
            }

            return end - start;
        }
    }
}
