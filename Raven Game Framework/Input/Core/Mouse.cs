using System;
using System.Drawing;
using System.Threading.Atomics;
using JoshuaKearney.Collections;
using SFML.Window;

namespace Raven.Input.Core {
    public class Mouse : AbstractMouse {
        //vars
        public override event EventHandler<MouseMoveEventArgs> MouseMove = null;
        public override event EventHandler<MouseWheelScrollEventArgs> MouseWheel = null;
        public override event EventHandler<MouseButtonEventArgs> MouseDown = null;
        public override event EventHandler<MouseButtonEventArgs> MouseUp = null;

        private ConcurrentSet<Display.Window> windows = new ConcurrentSet<Display.Window>();

        //constructor
        internal Mouse(AtomicBoolean usingController) : base(usingController) {

        }

        //public

        //private
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
                currentWindow = null;
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

            point = new Point(e.X, e.Y);
            currentWindow = (Display.Window) sender;
            MouseMove?.Invoke(sender, e);
        }
        protected virtual void OnMouseWheel(object sender, MouseWheelScrollEventArgs e) {
            usingController.Value = false;

            wheelDelta = e.Delta;
            currentWindow = (Display.Window) sender;
            MouseWheel?.Invoke(sender, e);
        }
        protected virtual void OnMouseDown(object sender, MouseButtonEventArgs e) {
            usingController.Value = false;

            if (e.Button == SFML.Window.Mouse.Button.Left) {
                left = true;
            } else if (e.Button == SFML.Window.Mouse.Button.Middle) {
                middle = true;
            } else if (e.Button == SFML.Window.Mouse.Button.Right) {
                right = true;
            } else if (e.Button == SFML.Window.Mouse.Button.XButton1) {
                extra1 = true;
            } else if (e.Button == SFML.Window.Mouse.Button.XButton2) {
                extra2 = true;
            }

            currentWindow = (Display.Window) sender;
            MouseDown?.Invoke(sender, e);
        }
        protected virtual void OnMouseUp(object sender, MouseButtonEventArgs e) {
            usingController.Value = false;

            if (e.Button == SFML.Window.Mouse.Button.Left) {
                left = false;
            } else if (e.Button == SFML.Window.Mouse.Button.Middle) {
                middle = false;
            } else if (e.Button == SFML.Window.Mouse.Button.Right) {
                right = false;
            } else if (e.Button == SFML.Window.Mouse.Button.XButton1) {
                extra1 = false;
            } else if (e.Button == SFML.Window.Mouse.Button.XButton2) {
                extra2 = false;
            }

            currentWindow = (Display.Window) sender;
            MouseUp?.Invoke(sender, e);
        }
    }
}
