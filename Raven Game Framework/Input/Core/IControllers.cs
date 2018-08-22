using Raven.Geom;
using Raven.Input.Enums;
using Raven.Input.Events;
using System;

namespace Raven.Input.Core {
    public interface IControllers {
        //functions
        event EventHandler<ButtonEventArgs> ButtonDown;
        event EventHandler<ButtonEventArgs> ButtonUp;
        event EventHandler<StickEventArgs> StickMoved;
        event EventHandler<TriggerEventArgs> TriggerPressed;

        bool Supported { get; set; }

        double StickDeadzone { get; set; }
        double TriggerDeadzone { get; set; }

        int ConnectedControllers { get; }

        bool IsAnyCodeFlagged(int controller, params int[] codes);
        bool AreAllCodesFlagged(int controller, params int[] codes);
        double GetTriggerPressure(int controller, XboxSide side);
        PointD GetStickPosition(int controller, XboxSide side);
        void Vibrate(int controller, double leftIntensity, double rightIntensity);
    }
}
