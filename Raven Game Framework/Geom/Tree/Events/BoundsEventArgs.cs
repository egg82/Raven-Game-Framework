using System;

namespace Raven.Geom.Tree.Events {
    public class BoundsEventArgs : EventArgs {
        //vars
        public static readonly new BoundsEventArgs Empty = new BoundsEventArgs(0.0d, 0.0d, 1.0d, 1.0d);

        private readonly double x = 0.0d;
        private readonly double y = 0.0d;
        private readonly double width = 0.0d;
        private readonly double height = 0.0d;

        //constructor
        public BoundsEventArgs(double x, double y, double width, double height) {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        //public
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
        public double Width {
            get {
                return width;
            }
        }
        public double Height {
            get {
                return height;
            }
        }

        //private

    }
}
