using System;
using System.Threading.Atomics;
using SFML.Window;
using static SFML.Window.Keyboard;

namespace Raven.Input.Core {
    public abstract class AbstractKeyboard : IKeyboard {
        //vars
        public abstract event EventHandler<KeyEventArgs> KeyUp;
        public abstract event EventHandler<KeyEventArgs> KeyDown;

        protected volatile bool[] keys = new bool[256];
        protected AtomicBoolean usingController = null;

        //constructor
        internal AbstractKeyboard(AtomicBoolean usingController) {
            this.usingController = usingController;
        }

        //public
        public virtual bool IsAnyKeyDown(params Key[] keyCodes) {
            if (keyCodes == null || keyCodes.LongLength == 0L) {
                return true;
            }

            foreach (int k in keyCodes) {
                if (k < 0 || k >= keys.Length) {
                    continue;
                }
                if (keys[k]) {
                    return true;
                }
            }

            return false;
        }
        public virtual bool AreAllKeysDown(params Key[] keyCodes) {
            if (keyCodes == null || keyCodes.LongLength == 0L) {
                return true;
            }

            foreach (int k in keyCodes) {
                if (k < 0 || k >= keys.Length || !keys[k]) {
                    return false;
                }
            }

            return true;
        }

        //private
        internal abstract void AddWindow(Display.Window window);
        internal abstract void RemoveWindow(Display.Window window);
    }
}
