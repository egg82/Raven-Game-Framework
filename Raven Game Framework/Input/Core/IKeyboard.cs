using SFML.Window;
using System;

namespace Raven.Input.Core {
    public interface IKeyboard {
        //functions
        event EventHandler<KeyEventArgs> KeyUp;
        event EventHandler<KeyEventArgs> KeyDown;

        bool IsAnyKeyDown(params int[] keyCodes);
        bool AreAllKeysDown(params int[] keyCodes);
    }
}
