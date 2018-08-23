using SFML.Graphics;
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

        public event EventHandler<EventArgs> GainedFocus = null;
        public event EventHandler<EventArgs> LostFocus = null;
        public event EventHandler<EventArgs> Closed = null;

        private RenderWindow window = null;
        private readonly object windowLock = new object();
        private readonly object updateLock = new object();
        private readonly object drawLock = new object();

        private string title = null;

        //constructor
        public Window(uint width, uint height, string title, Styles style = Styles.Default, bool vsync = true, ushort antialiasing = 16) {
            window = new RenderWindow(new VideoMode(width, height), title, style, new ContextSettings(24, 8, antialiasing));
            this.title = title;

            window.KeyPressed += OnKeyPressed;
            window.KeyReleased += OnKeyReleased;

            window.MouseMoved += OnMouseMoved;
            window.MouseWheelScrolled += OnMouseWheel;
            window.MouseButtonPressed += OnMouseDown;
            window.MouseButtonReleased += OnMouseUp;

            window.GainedFocus += OnGainedFocus;
            window.LostFocus += OnLostFocus;
            window.Closed += OnClosed;

            window.SetVerticalSyncEnabled(vsync);
            window.SetActive(false);
        }

        //public
        public string Title {
            get {
                return title;
            }
            set {
                if (value == null || value == title) {
                    return;
                }

                title = value;
                window.SetTitle(title);
            }
        }
        public void Close() {
            window.Close();
        }

        public void UpdateWindow() {
            lock (windowLock) {
                window.DispatchEvents();
            }
        }
        public void UpdateStates(double deltaTime) {
            lock (updateLock) {

            }
        }
        public void DrawGraphics() {
            lock (drawLock) {
                window.Clear(Color.Transparent);
                window.Display();
                window.SetActive(false);
            }
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
        private void OnLostFocus(object sender, EventArgs e) {
            LostFocus?.Invoke(this, e);
        }
        private void OnClosed(object sender, EventArgs e) {
            Closed?.Invoke(this, e);
        }
    }
}
