using log4net;
using SFML.Window;
using System.Reflection;
using System.Threading.Atomics;

namespace Raven.Input.Core {
    public class LoggingKeyboard : Keyboard {
        //vars
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        //constructor
        internal LoggingKeyboard(AtomicBoolean usingController) : base(usingController) {
            log.Debug("Keyboard handler created.");
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

        protected override void OnKeyDown(object sender, KeyEventArgs e) {
            log.Debug("KeyUp code=" + e.Code + ", alt=" + e.Alt + ", ctrl=" + e.Control + ", shift=" + e.Shift + ", system=" + e.System);
            base.OnKeyUp(sender, e);
        }
        protected override void OnKeyUp(object sender, KeyEventArgs e) {
            log.Debug("KeyDown code=" + e.Code + ", alt=" + e.Alt + ", ctrl=" + e.Control + ", shift=" + e.Shift + ", system=" + e.System);
            base.OnKeyUp(sender, e);
        }
    }
}
