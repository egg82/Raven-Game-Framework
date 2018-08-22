using System;
using System.Threading.Atomics;
using JoshuaKearney.Collections;
using SFML.Window;

namespace Raven.Input.Core {
    public class Keyboard : AbstractKeyboard {
        //vars
        public override event EventHandler<KeyEventArgs> KeyUp = null;
        public override event EventHandler<KeyEventArgs> KeyDown = null;

        private ConcurrentSet<Display.Window> windows = new ConcurrentSet<Display.Window>();

        //constructor
        internal Keyboard(AtomicBoolean usingController) : base(usingController) {

        }

        //public

        //private
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

            if (keys[key]) {
                return;
            }
            keys[key] = true;

            KeyDown?.Invoke(sender, e);
        }
        protected virtual void OnKeyUp(object sender, KeyEventArgs e) {
            usingController.Value = false;

            int key = (int) e.Code;
            if (key < 0 || key > keys.Length) {
                return;
            }

            if (!keys[key]) {
                return;
            }
            keys[key] = false;

            KeyUp?.Invoke(sender, e);
        }
    }
}
