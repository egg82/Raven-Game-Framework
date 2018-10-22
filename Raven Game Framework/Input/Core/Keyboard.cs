using System;
using System.Threading;
using System.Threading.Atomics;
using JoshuaKearney.Collections;
using SFML.Window;

namespace Raven.Input.Core {
    public class Keyboard : AbstractKeyboard {
        // events
        public override event EventHandler<KeyEventArgs> KeyUp = null;
        public override event EventHandler<KeyEventArgs> KeyDown = null;

        // vars
        private readonly ConcurrentSet<Display.Window> windows = new ConcurrentSet<Display.Window>();

        // constructor
        internal Keyboard(AtomicBoolean usingController) : base(usingController) {

        }

        // public

        // private
        internal override void AddWindow(Display.Window window) {
            if (!windows.Add(window)) {
                return;
            }

            window.KeyPressed += OnKeyDown;
            window.KeyReleased += OnKeyUp;
        }
        internal override void RemoveWindow(Display.Window window) {
            if (!windows.Remove(window)) {
                return;
            }

            window.KeyPressed -= OnKeyDown;
            window.KeyReleased -= OnKeyUp;
        }

        protected virtual void OnKeyDown(object sender, KeyEventArgs e) {
            usingController.Value = false;

            int key = (int) e.Code;
            if (key < 0 || key > keys.Length) {
                return;
            }

            if (keys[key] != 0) {
                return;
            }
            Interlocked.Exchange(ref keys[key], 1);

            KeyDown?.Invoke(sender, e);
        }
        protected virtual void OnKeyUp(object sender, KeyEventArgs e) {
            usingController.Value = false;

            int key = (int) e.Code;
            if (key < 0 || key > keys.Length) {
                return;
            }

            if (keys[key] == 0) {
                return;
            }
            Interlocked.Exchange(ref keys[key], 0);

            KeyUp?.Invoke(sender, e);
        }
    }
}
