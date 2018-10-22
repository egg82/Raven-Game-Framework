using System;
using System.Threading;
using System.Threading.Atomics;
using JoshuaKearney.Collections;
using SFML.Window;

namespace Raven.Input.Core {
    public class Mouse : AbstractMouse {
        // events
        public override event EventHandler<MouseMoveEventArgs> MouseMove = null;
        public override event EventHandler<MouseWheelScrollEventArgs> MouseWheel = null;
        public override event EventHandler<MouseButtonEventArgs> MouseDown = null;
        public override event EventHandler<MouseButtonEventArgs> MouseUp = null;

        // vars
        private readonly ConcurrentSet<Display.Window> windows = new ConcurrentSet<Display.Window>();

        // constructor
        internal Mouse(AtomicBoolean usingController) : base(usingController) {

        }

        // public

        // private
        internal override void AddWindow(Display.Window window) {
            if (!windows.Add(window)) {
                return;
            }

            window.MouseMoved += OnMouseMove;
            window.MouseWheelScrolled += OnMouseWheel;
            window.MouseButtonPressed += OnMouseDown;
            window.MouseButtonReleased += OnMouseUp;
        }
        internal override void RemoveWindow(Display.Window window) {
            if (currentWindow == window) {
                Interlocked.Exchange(ref currentWindow, null);
            }

            if (!windows.Remove(window)) {
                return;
            }

            window.MouseMoved -= OnMouseMove;
            window.MouseWheelScrolled -= OnMouseWheel;
            window.MouseButtonPressed -= OnMouseDown;
            window.MouseButtonReleased -= OnMouseUp;
        }

        protected virtual void OnMouseMove(object sender, MouseMoveEventArgs e) {
            usingController.Value = false;

            Interlocked.Exchange(ref xy, ((long) e.X << 32) | (uint) e.Y);
            Interlocked.Exchange(ref currentWindow, (Display.Window) sender);
            MouseMove?.Invoke(sender, e);
        }
        protected virtual void OnMouseWheel(object sender, MouseWheelScrollEventArgs e) {
            usingController.Value = false;

            Interlocked.Exchange(ref wheelDelta, e.Delta);
            Interlocked.Exchange(ref currentWindow, (Display.Window) sender);
            MouseWheel?.Invoke(sender, e);
        }
        protected virtual void OnMouseDown(object sender, MouseButtonEventArgs e) {
            usingController.Value = false;

            if (e.Button == SFML.Window.Mouse.Button.Left) {
                Interlocked.Exchange(ref left, 1);
            } else if (e.Button == SFML.Window.Mouse.Button.Middle) {
                Interlocked.Exchange(ref middle, 1);
            } else if (e.Button == SFML.Window.Mouse.Button.Right) {
                Interlocked.Exchange(ref right, 1);
            } else if (e.Button == SFML.Window.Mouse.Button.XButton1) {
                Interlocked.Exchange(ref extra1, 1);
            } else if (e.Button == SFML.Window.Mouse.Button.XButton2) {
                Interlocked.Exchange(ref extra2, 1);
            }

            Interlocked.Exchange(ref currentWindow, (Display.Window) sender);
            MouseDown?.Invoke(sender, e);
        }
        protected virtual void OnMouseUp(object sender, MouseButtonEventArgs e) {
            usingController.Value = false;

            if (e.Button == SFML.Window.Mouse.Button.Left) {
                Interlocked.Exchange(ref left, 0);
            } else if (e.Button == SFML.Window.Mouse.Button.Middle) {
                Interlocked.Exchange(ref middle, 0);
            } else if (e.Button == SFML.Window.Mouse.Button.Right) {
                Interlocked.Exchange(ref right, 0);
            } else if (e.Button == SFML.Window.Mouse.Button.XButton1) {
                Interlocked.Exchange(ref extra1, 0);
            } else if (e.Button == SFML.Window.Mouse.Button.XButton2) {
                Interlocked.Exchange(ref extra2, 0);
            }

            Interlocked.Exchange(ref currentWindow, (Display.Window) sender);
            MouseUp?.Invoke(sender, e);
        }
    }
}
