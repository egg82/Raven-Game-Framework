using Raven.Geom.Tree.Events;
using Raven.Utils;
using System;

namespace Raven.Geom.Tree {
    public abstract class QuadNode {
        //vars
        internal event EventHandler<BoundsEventArgs> BoundsChanged = null;

        private double x = 0.0d;
        private double y = 0.0d;
        private double width = 1.0d;
        private double height = 1.0d;

        //constructor
        public QuadNode() {

        }
        public QuadNode(PointD point, SizeD size) {
            if (point == null) {
                throw new ArgumentNullException("point");
            }
            if (size == null) {
                throw new ArgumentNullException("size");
            }

            x = MathUtil.Clamp(double.MinValue, double.MaxValue, point.X);
            y = MathUtil.Clamp(double.MinValue, double.MaxValue, point.Y);
            width = MathUtil.Clamp(double.MinValue, double.MaxValue, size.Width);
            height = MathUtil.Clamp(double.MinValue, double.MaxValue, size.Height);
        }
        public QuadNode(double x, double y, double width, double height) {
            this.x = MathUtil.Clamp(double.MinValue, double.MaxValue, x);
            this.y = MathUtil.Clamp(double.MinValue, double.MaxValue, y);
            this.width = MathUtil.Clamp(double.MinValue, double.MaxValue, width);
            this.height = MathUtil.Clamp(double.MinValue, double.MaxValue, height);
        }

        //public
        public virtual double X {
            get {
                return x;
            }
            set {
                if (value == x) {
                    return;
                }
                if (double.IsNaN(value) || double.IsInfinity(value)) {
                    throw new NotFiniteNumberException(value);
                }

                x = value;
                BoundsChanged?.Invoke(this, new BoundsEventArgs(x, y, width, height));
            }
        }
        public virtual double Y {
            get {
                return y;
            }
            set {
                if (value == y) {
                    return;
                }
                if (double.IsNaN(value) || double.IsInfinity(value)) {
                    throw new NotFiniteNumberException(value);
                }

                y = value;
                BoundsChanged?.Invoke(this, new BoundsEventArgs(x, y, width, height));
            }
        }
        public virtual double Width {
            get {
                return width;
            }
            set {
                if (value == width) {
                    return;
                }
                if (value <= 0.0d) {
                    throw new Exception("value must be positive and non-zero.");
                }
                if (double.IsNaN(value) || double.IsInfinity(value)) {
                    throw new NotFiniteNumberException(value);
                }

                width = value;
                BoundsChanged?.Invoke(this, new BoundsEventArgs(x, y, width, height));
            }
        }
        public virtual double Height {
            get {
                return height;
            }
            set {
                if (value == height) {
                    return;
                }
                if (value <= 0.0d) {
                    throw new Exception("value must be positive and non-zero.");
                }
                if (double.IsNaN(value) || double.IsInfinity(value)) {
                    throw new NotFiniteNumberException(value);
                }

                height = value;
                BoundsChanged?.Invoke(this, new BoundsEventArgs(x, y, width, height));
            }
        }

        //private

    }
}
