using Raven.Input.Enums;
using System;

namespace Raven.Input.Events {
    public class StickEventArgs : EventArgs {
        // vars
        public static readonly new StickEventArgs Empty = new StickEventArgs(-1, XboxSide.None, XboxStickDirection.None, 0.0d, 0.0d);

        // constructor
        public StickEventArgs(int controller, XboxSide side, XboxStickDirection direction, double x, double y) {
            Controller = controller;
            Side = side;
            Direction = direction;
            X = x;
            Y = y;
        }

        // public
        public int Controller { get; private set; }
        public XboxSide Side { get; private set; }
        public XboxStickDirection Direction { get; private set; }
        public double X { get; private set; }
        public double Y { get; private set; }

        // private

    }
}
