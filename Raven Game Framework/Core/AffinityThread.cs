using Raven.Utils;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Atomics;

namespace Raven.Core {
    public class AffinityThread {
        //vars
        [DllImport("kernel32.dll")]
        private static extern int GetCurrentThreadId();

        private int core = 0;
        private Thread thread = null;
        private readonly object threadLock = new object();

        private ThreadStart start = null;
        private ThreadPriority priority = ThreadPriority.Normal;
        private readonly int maxStackSize = 0;

        private AtomicBoolean running = new AtomicBoolean(false);

        //constructor
        public AffinityThread(ThreadStart start, ThreadPriority priority = ThreadPriority.Normal, int maxStackSize = 0) {
            if (start == null) {
                throw new ArgumentNullException("start");
            }

            this.start = start;
            this.priority = priority;
            this.maxStackSize = maxStackSize;
        }

        //public
        public int ProcessorCore {
            get {
                return core;
            }
            set {
                if (value == core) {
                    return;
                }

                core = MathUtil.Clamp(0, Environment.ProcessorCount, value);
                Init();
            }
        }
        public ThreadPriority Priority {
            get {
                return priority;
            }
            set {
                if (priority == value) {
                    return;
                }

                priority = value;
                Init();
            }
        }

        public void Start() {
            if (running.CompareExchange(true, false)) {
                return;
            }

            Init();
        }
        public void Abort() {
            if (!running.CompareExchange(false, true)) {
                return;
            }

            thread.Abort();
        }
        public System.Threading.ThreadState State {
            get {
                return thread.ThreadState;
            }
        }

        //private
        private void Init() {
            lock (threadLock) {
                bool previouslyRunning = running.CompareExchange(false, true);

                if (previouslyRunning) {
                    thread?.Abort();
                }
                thread = new Thread(delegate() {
                    ProcessThread currentThread = getCurrentThread();
                    int currentCore = core;

                    if (currentCore > 0) {
                        Thread.BeginThreadAffinity();
                        currentThread.ProcessorAffinity = new IntPtr(1 << currentCore - 1);
                    }

                    start.Invoke();
                    
                    if (currentCore > 0) {
                        Thread.EndThreadAffinity();
                    }
                    running.Value = false;
                }, maxStackSize) {
                    Priority = priority
                };

                if (previouslyRunning) {
                    running.Value = true;
                    thread.Start();
                }
            }
        }
        private ProcessThread getCurrentThread() {
            return (from ProcessThread thread in Process.GetCurrentProcess().Threads where thread.Id == GetCurrentThreadId() select thread).Single();
        }
    }
}
