using Raven.Input.Enums;
using System;

namespace Raven.Input.Events {
    public class StickEventArgs : EventArgs {
        //vars
        public static readonly new StickEventArgs Empty = new StickEventArgs(-1, XboxSide.None, XboxStickDirection.None, 0.0d, 0.0d);

        private readonly int controller = -1;
        private readonly XboxSide side = XboxSide.None;
        private readonly XboxStickDirection direction = XboxStickDirection.None;
        private readonly double x = 0.0d;
        private readonly double y = 0.0d;

        //constructor
        public StickEventArgs(int controller, XboxSide side, XboxStickDirection direction, double x, double y) {
            this.controller = controller;
            this.side = side;
            this.direction = direction;
            this.x = x;
            this.y = y;
        }

        //public
        public int Controller {
            get {
                return controller;
            }
        }
        public XboxSide Side {
            get {
                return side;
            }
        }
        public XboxStickDirection Direction {
            get {
                return direction;
            }
        }
        public double X {
            get {
                return x;
            }
        }
        public double Y {
            get {
                return y;
            }
        }

        //private

    }
}
