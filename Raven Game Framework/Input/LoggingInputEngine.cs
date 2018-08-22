using log4net;
using Raven.Display;
using System;
using System.Reflection;

namespace Raven.Input {
    public class LoggingInputEngine : InputEngine {
        //vars
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        //constructor
        public LoggingInputEngine() : base(new Core.LoggingMouse(usingController), new Core.LoggingKeyboard(usingController), new Core.LoggingControllers(usingController)) {
            log.Debug("Input engine enabled.");
        }

        //public
        public override void AddWindow(Window window) {
            log.Debug("Adding window \"" + window.Title + "\"");
            base.AddWindow(window);
        }
        public override void RemoveWindow(Window window) {
            log.Debug("Removing window \"" + window.Title + "\"");
            base.RemoveWindow(window);
        }

        //private
        protected override void OnFocused(object sender, EventArgs e) {
            log.Debug("Window took focus \"" + ((Window) sender).Title + "\"");
        }
    }
}
