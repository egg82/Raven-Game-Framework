using SFML.Window;
using System;
using static SFML.Window.Keyboard;

namespace Raven.Input.Core {
    public interface IKeyboard {
        event EventHandler<KeyEventArgs> KeyUp;
        event EventHandler<KeyEventArgs> KeyDown;

        bool IsAnyKeyDown(params Key[] keyCodes);
        bool AreAllKeysDown(params Key[] keyCodes);
    }
}
