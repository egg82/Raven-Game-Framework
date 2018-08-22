using JoshuaKearney.Collections;
using Raven.Display;
using Raven.Input.Core;
using System;

namespace Raven.Input {
    public class InputEngine : AbstractInputEngine {
        //vars
        private ConcurrentSet<Window> windows = new ConcurrentSet<Window>();
        private volatile Window currentWindow = null;

        //constructor
        public InputEngine() : base(new Mouse(usingController), new Keyboard(usingController), new Controllers(usingController)) {

        }
        internal InputEngine(AbstractMouse mouse, AbstractKeyboard keyboard, AbstractControllers controllers) : base(mouse, keyboard, controllers) {

        }

        //public
        public Window CurrentWindow {
            get {
                return currentWindow;
            }
        }

        public override void AddWindow(Window window) {
            if (window == null) {
                throw new ArgumentNullException("window");
            }

            mouse.AddWindow(window);
            keyboard.AddWindow(window);

            if (!windows.Add(window)) {
                return;
            }
            if (currentWindow == null) {
                currentWindow = window;
            }

            window.GainedFocus += OnFocused;
        }
        public override void RemoveWindow(Window window) {
            if (window == null) {
                throw new ArgumentNullException("window");
            }

            mouse.RemoveWindow(window);
            keyboard.RemoveWindow(window);

            if (!windows.Remove(window)) {
                return;
            }
            if (currentWindow == window) {
                currentWindow = null;
            }

            window.GainedFocus -= OnFocused;
        }

        public override void UpdateControllers() {
            controllers.Update(currentWindow);
        }

        //private
        protected virtual void OnFocused(object sender, EventArgs e) {
            currentWindow = (Window) sender;
        }
    }
}
