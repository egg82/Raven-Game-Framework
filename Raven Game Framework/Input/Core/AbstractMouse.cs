using SFML.Window;
using System;
using System.Drawing;
using System.Threading;
using System.Threading.Atomics;

namespace Raven.Input.Core {
    public abstract class AbstractMouse : IMouse {
        // events
        public abstract event EventHandler<MouseMoveEventArgs> MouseMove;
        public abstract event EventHandler<MouseWheelScrollEventArgs> MouseWheel;
        public abstract event EventHandler<MouseButtonEventArgs> MouseDown;
        public abstract event EventHandler<MouseButtonEventArgs> MouseUp;

        // vars
        protected Display.Window currentWindow = null;
        protected double wheelDelta = 0.0d;

        protected long xy = 0L;

        protected int left = 0;
        protected int middle = 0;
        protected int right = 0;
        protected int extra1 = 0;
        protected int extra2 = 0;

        protected AtomicBoolean usingController = null;

        // constructor
        protected AbstractMouse(AtomicBoolean usingController) {
            this.usingController = usingController;
        }

        // public
        public virtual Display.Window CurrentWindow {
            get {
                return currentWindow;
            }
        }
        public virtual double WheelDelta {
            get {
                return wheelDelta;
            }
        }
        public virtual int X {
            get {
                long current = Interlocked.Read(ref xy);
                return (int) (current >> 32);
            }
        }
        public virtual int Y {
            get {
                long current = Interlocked.Read(ref xy);
                return (int) (current & 0xffffffff);
            }
        }

        public virtual bool LeftDown {
            get {
                return left != 0;
            }
        }
        public virtual bool MiddleDown {
            get {
                return middle != 0;
            }
        }
        public virtual bool RightDown {
            get {
                return right != 0;
            }
        }
        public virtual bool Extra1Down {
            get {
                return extra1 != 0;
            }
        }
        public virtual bool Extra2Down {
            get {
                return extra1 != 0;
            }
        }

        // private
        internal abstract void AddWindow(Display.Window window);
        internal abstract void RemoveWindow(Display.Window window);
    }
}
