using log4net;
using Raven.Display;
using Raven.Input.Events;
using System.Reflection;
using System.Threading.Atomics;

namespace Raven.Input.Core {
    public class LoggingControllers : Controllers {
        //vars
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        //constructor
        internal LoggingControllers(AtomicBoolean usingController) : base(usingController) {
            log.Debug("Controllers handler created.");

            ButtonDown += OnButtonDown;
            ButtonUp += OnButtonUp;
            StickMoved += OnStickMoved;
            TriggerPressed += OnTriggerPressed;
        }

        //public
        public override double StickDeadzone {
            get {
                return base.StickDeadzone;
            }
            set {
                log.Debug("Attempting to set (stick) deadzone to " + value);
                base.StickDeadzone = value;
            }
        }
        public override double TriggerDeadzone {
            get {
                return base.TriggerDeadzone;
            }
            set {
                log.Debug("Attempting to set (trigger) deadzone to " + value);
                base.TriggerDeadzone = value;
            }
        }

        public override void Vibrate(int controller, double leftIntensity, double rightIntensity) {
            log.Debug("Vibrating controller #" + controller + " with intensity left=" + leftIntensity + ", right=" + rightIntensity);
            base.Vibrate(controller, leftIntensity, rightIntensity);
        }

        //private
        private void OnButtonDown(object sender, ButtonEventArgs e) {
            log.Debug("ButtonDown controller #" + e.Controller + " with code=" + e.Code);
        }
        private void OnButtonUp(object sender, ButtonEventArgs e) {
            log.Debug("ButtonUp controller #" + e.Controller + " with code=" + e.Code);
        }
        private void OnStickMoved(object sender, StickEventArgs e) {
            log.Debug("StickMoved controller #" + e.Controller + " with side=" + e.Side + ", direction=" + e.Direction + ", x=" + e.X + ", y=" + e.Y);
        }
        private void OnTriggerPressed(object sender, TriggerEventArgs e) {
            log.Debug("TriggerPressed controller #" + e.Controller + " with side=" + e.Side + ", pressure=" + e.Pressure);
        }
    }
}
