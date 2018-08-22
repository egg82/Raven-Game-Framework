using SFML.Window;
using System;
using System.Drawing;
using System.Threading.Atomics;

namespace Raven.Input.Core {
    public abstract class AbstractMouse : IMouse {
        //vars
        public abstract event EventHandler<MouseMoveEventArgs> MouseMove;
        public abstract event EventHandler<MouseWheelScrollEventArgs> MouseWheel;
        public abstract event EventHandler<MouseButtonEventArgs> MouseDown;
        public abstract event EventHandler<MouseButtonEventArgs> MouseUp;

        protected volatile Display.Window currentWindow = null;
        protected double wheelDelta = 0.0d;
        protected Point point = new Point(0, 0);

        protected volatile bool left = false;
        protected volatile bool middle = false;
        protected volatile bool right = false;
        protected volatile bool extra1 = false;
        protected volatile bool extra2 = false;

        protected AtomicBoolean usingController = null;

        //constructor
        internal AbstractMouse(AtomicBoolean usingController) {
            this.usingController = usingController;
        }

        //public
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
        public virtual Point Point {
            get {
                return point;
            }
        }

        public virtual bool LeftDown {
            get {
                return left;
            }
        }
        public virtual bool MiddleDown {
            get {
                return middle;
            }
        }
        public virtual bool RightDown {
            get {
                return right;
            }
        }
        public virtual bool Extra1Down {
            get {
                return extra1;
            }
        }
        public virtual bool Extra2Down {
            get {
                return extra1;
            }
        }

        //private
        internal abstract void AddWindow(Display.Window window);
        internal abstract void RemoveWindow(Display.Window window);
    }
}
