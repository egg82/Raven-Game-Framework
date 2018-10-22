using System;
using System.Threading.Atomics;
using SFML.Window;
using static SFML.Window.Keyboard;

namespace Raven.Input.Core {
    public abstract class AbstractKeyboard : IKeyboard {
        // events
        public abstract event EventHandler<KeyEventArgs> KeyUp;
        public abstract event EventHandler<KeyEventArgs> KeyDown;

        // vars
        protected int[] keys = new int[256];
        protected AtomicBoolean usingController = null;

        // constructor
        protected AbstractKeyboard(AtomicBoolean usingController) {
            this.usingController = usingController;
        }

        // public
        public virtual bool IsAnyKeyDown(params Key[] keyCodes) {
            if (keyCodes == null || keyCodes.LongLength == 0L) {
                return true;
            }

            foreach (Key k in keyCodes) {
                if (k < 0 || (int) k >= keys.Length) {
                    continue;
                }
                if (keys[(int) k] != 0) {
                    return true;
                }
            }

            return false;
        }
        public virtual bool AreAllKeysDown(params Key[] keyCodes) {
            if (keyCodes == null || keyCodes.LongLength == 0L) {
                return true;
            }

            foreach (Key k in keyCodes) {
                if (k < 0 || (int) k >= keys.Length || keys[(int) k] == 0) {
                    return false;
                }
            }

            return true;
        }

        // private
        internal abstract void AddWindow(Display.Window window);
        internal abstract void RemoveWindow(Display.Window window);
    }
}
