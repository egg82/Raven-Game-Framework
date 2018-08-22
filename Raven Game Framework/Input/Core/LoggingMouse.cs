using log4net;
using SFML.Window;
using System.Reflection;
using System.Threading.Atomics;

namespace Raven.Input.Core {
    public class LoggingMouse : Mouse {
        //vars
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        //constructor
        internal LoggingMouse(AtomicBoolean usingController) : base(usingController) {
            log.Debug("Mouse handler created.");
        }

        //public

        //private
        internal override void AddWindow(Display.Window window) {
            log.Debug("Adding window \"" + window.Title + "\"");
            base.AddWindow(window);
        }
        internal override void RemoveWindow(Display.Window window) {
            log.Debug("Removing window \"" + window.Title + "\"");
            base.RemoveWindow(window);
        }

        protected override void OnMouseMove(object sender, MouseMoveEventArgs e) {
            log.Debug("MouseMove X=" + e.X + ", Y=" + e.Y);
            base.OnMouseMove(sender, e);
        }
        protected override void OnMouseWheel(object sender, MouseWheelScrollEventArgs e) {
            log.Debug("MouseWheel delta=" + e.Delta);
            base.OnMouseWheel(sender, e);
        }
        protected override void OnMouseDown(object sender, MouseButtonEventArgs e) {
            log.Debug("MouseDown button=" + e.Button);
            base.OnMouseDown(sender, e);
        }
        protected override void OnMouseUp(object sender, MouseButtonEventArgs e) {
            log.Debug("MouseDown up=" + e.Button);
            base.OnMouseUp(sender, e);
        }
    }
}
