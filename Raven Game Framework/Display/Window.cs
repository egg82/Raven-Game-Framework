using SFML.Window;
using System;

namespace Raven.Display {
    public class Window {
        //vars
        internal event EventHandler<KeyEventArgs> KeyPressed = null;
        internal event EventHandler<KeyEventArgs> KeyReleased = null;

        internal event EventHandler<MouseMoveEventArgs> MouseMoved = null;
        internal event EventHandler<MouseWheelScrollEventArgs> MouseWheelScrolled = null;
        internal event EventHandler<MouseButtonEventArgs> MouseButtonPressed = null;
        internal event EventHandler<MouseButtonEventArgs> MouseButtonReleased = null;

        internal event EventHandler<EventArgs> GainedFocus = null;

        private SFML.Window.Window window = null;

        private string title = null;

        //constructor
        public Window(string title) {
            window = new SFML.Window.Window(new VideoMode(800, 600), title);
            this.title = title;

            window.KeyPressed += OnKeyPressed;
            window.KeyReleased += OnKeyReleased;

            window.MouseMoved += OnMouseMoved;
            window.MouseWheelScrolled += OnMouseWheel;
            window.MouseButtonPressed += OnMouseDown;
            window.MouseButtonReleased += OnMouseUp;

            window.GainedFocus += OnGainedFocus;
        }

        //public
        public string Title {
            get {
                return title;
            }
        }

        public void Update() {
            window.DispatchEvents();
        }
        public void Draw() {
            window.Display();
        }

        //private
        private void OnKeyPressed(object sender, KeyEventArgs e) {
            KeyPressed?.Invoke(this, e);
        }
        private void OnKeyReleased(object sender, KeyEventArgs e) {
            KeyReleased?.Invoke(this, e);
        }

        private void OnMouseMoved(object sender, MouseMoveEventArgs e) {
            MouseMoved?.Invoke(this, e);
        }
        private void OnMouseWheel(object sender, MouseWheelScrollEventArgs e) {
            MouseWheelScrolled?.Invoke(this, e);
        }
        private void OnMouseDown(object sender, MouseButtonEventArgs e) {
            MouseButtonPressed?.Invoke(this, e);
        }
        private void OnMouseUp(object sender, MouseButtonEventArgs e) {
            MouseButtonReleased?.Invoke(this, e);
        }

        private void OnGainedFocus(object sender, EventArgs e) {
            GainedFocus?.Invoke(this, e);
        }
    }
}
