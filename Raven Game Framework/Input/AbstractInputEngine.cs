using Raven.Input.Core;
using System.Threading.Atomics;

namespace Raven.Input {
    public abstract class AbstractInputEngine : IInputEngine {
        // vars
        internal readonly AbstractMouse mouse = null;
        internal readonly AbstractKeyboard keyboard = null;
        internal readonly AbstractControllers controllers = null;

        protected static readonly AtomicBoolean usingController = new AtomicBoolean(false);

        // constructor
        protected AbstractInputEngine(AbstractMouse mouse, AbstractKeyboard keyboard, AbstractControllers controllers) {
            this.mouse = mouse;
            this.keyboard = keyboard;
            this.controllers = controllers;
        }

        // public
        public IMouse Mouse {
            get {
                return mouse;
            }
        }
        public IKeyboard Keyboard {
            get {
                return keyboard;
            }
        }
        public IControllers Controllers {
            get {
                return controllers;
            }
        }

        public bool UsingController {
            get {
                return usingController.Value;
            }
        }

        public abstract void AddWindow(Display.Window window);
        public abstract void RemoveWindow(Display.Window window);

        public abstract void UpdateControllers();

        // private

    }
}
