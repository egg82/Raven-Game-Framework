using Raven.Display;
using Raven.Geom;
using Raven.Input.Enums;
using Raven.Input.Events;
using System;
using System.Threading.Atomics;

namespace Raven.Input.Core {
    public abstract class AbstractControllers : IControllers {
        //vars
        public abstract event EventHandler<ButtonEventArgs> ButtonDown;
        public abstract event EventHandler<ButtonEventArgs> ButtonUp;
        public abstract event EventHandler<StickEventArgs> StickMoved;
        public abstract event EventHandler<TriggerEventArgs> TriggerPressed;

        private volatile bool supported = true;

        protected AtomicBoolean usingController = null;

        //constructor
        internal AbstractControllers(AtomicBoolean usingController) {
            this.usingController = usingController;
        }

        //public
        public bool Supported {
            get {
                return supported;
            }
            set {
                supported = value;
            }
        }
        
        public abstract double StickDeadzone { get; set; }
        public abstract double TriggerDeadzone { get; set; }
        public abstract int ConnectedControllers { get; }

        public abstract bool IsAnyCodeFlagged(int controller, params int[] codes);
        public abstract bool AreAllCodesFlagged(int controller, params int[] codes);
        public abstract double GetTriggerPressure(int controller, XboxSide side);
        public abstract PointD GetStickPosition(int controller, XboxSide side);
        public abstract void Vibrate(int controller, double leftIntensity, double rightIntensity);

        //private
        internal abstract void Update(Window window);
    }
}
