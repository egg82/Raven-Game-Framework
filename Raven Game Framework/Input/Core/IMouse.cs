using SFML.Window;
using System;
using System.Drawing;

namespace Raven.Input.Core {
    public interface IMouse {
        event EventHandler<MouseMoveEventArgs> MouseMove;
        event EventHandler<MouseWheelScrollEventArgs> MouseWheel;
        event EventHandler<MouseButtonEventArgs> MouseDown;
        event EventHandler<MouseButtonEventArgs> MouseUp;

        Display.Window CurrentWindow { get; }
        double WheelDelta { get; }
        int X { get; }
        int Y { get; }

        bool LeftDown { get; }
        bool MiddleDown { get; }
        bool RightDown { get; }
        bool Extra1Down { get; }
        bool Extra2Down { get; }
    }
}
