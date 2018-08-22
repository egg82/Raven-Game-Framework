using Raven.Input.Enums;
using System;

namespace Raven.Input.Events {
    public class TriggerEventArgs : EventArgs {
        //vars
        public static readonly new TriggerEventArgs Empty = new TriggerEventArgs(-1, XboxSide.None, 0.0d);

        private readonly int controller = -1;
        private readonly XboxSide side = XboxSide.None;
        private readonly double pressure = 0.0d;

        //constructor
        public TriggerEventArgs(int controller, XboxSide side, double pressure) {
            this.controller = controller;
            this.side = side;
            this.pressure = pressure;
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
        public double Pressure {
            get {
                return pressure;
            }
        }

        //private

    }
}
