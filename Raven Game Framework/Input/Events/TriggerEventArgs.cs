using Raven.Input.Enums;
using System;

namespace Raven.Input.Events {
    public class TriggerEventArgs : EventArgs {
        // vars
        public static readonly new TriggerEventArgs Empty = new TriggerEventArgs(-1, XboxSide.None, 0.0d);

        // constructor
        public TriggerEventArgs(int controller, XboxSide side, double pressure) {
            Controller = controller;
            Side = side;
            Pressure = pressure;
        }

        // public
        public int Controller { get; private set; }
        public XboxSide Side { get; private set; }
        public double Pressure { get; private set; }

        // private

    }
}
