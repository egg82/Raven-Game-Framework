using Raven.Input.Core;

namespace Raven.Input {
    public interface IInputEngine {
        IKeyboard Keyboard { get; }
        IMouse Mouse { get; }
        IControllers Controllers { get; }

        void AddWindow(Display.Window window);
        void RemoveWindow(Display.Window window);

        void UpdateControllers();
    }
}
