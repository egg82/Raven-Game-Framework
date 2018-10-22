using System;

namespace Raven.Geom.Tree.Events {
    public class BoundsEventArgs : EventArgs {
        // vars
        public static readonly new BoundsEventArgs Empty = new BoundsEventArgs(0.0d, 0.0d, 1.0d, 1.0d);

        // constructor
        public BoundsEventArgs(double x, double y, double width, double height) {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        // public
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Width { get; private set; }
        public double Height { get; private set; }

        // private

    }
}
