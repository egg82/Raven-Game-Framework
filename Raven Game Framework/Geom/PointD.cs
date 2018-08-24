using System;
using System.Drawing;

namespace Raven.Geom {
    public class PointD {
        //vars
        public static readonly PointD Empty = new PointD(0.0d, 0.0d);

        internal event EventHandler<EventArgs> Changed = null;

        private double x = 0.0d;
        private double y = 0.0d;

        //constructor
        public PointD() : this(0.0d, 0.0d) {

        }
        public PointD(double x, double y) {
            this.x = x;
            this.y = y;
        }

        //public
        public double X {
            get {
                return x;
            }
            set {
                if (value == x) {
                    return;
                }

                x = value;
                Changed?.Invoke(this, EventArgs.Empty);
            }
        }
        public double Y {
            get {
                return y;
            }
            set {
                if (value == y) {
                    return;
                }

                y = value;
                Changed?.Invoke(this, EventArgs.Empty);
            }
        }
        public bool IsEmpty {
            get {
                return (X == 0.0d && Y == 0.0d) ? true : false;
            }
        }

        public static PointD Add(PointD pt, SizeD sz) {
            return new PointD(pt.X + sz.Width, pt.Y + sz.Height);
        }
        public static PointD Add(PointD pt, SizeF sz) {
            return new PointD(pt.X + sz.Width, pt.Y + sz.Height);
        }
        public static PointD Add(PointD pt, Size sz) {
            return new PointD(pt.X + sz.Width, pt.Y + sz.Height);
        }

        public static PointD Subtract(PointD pt, SizeD sz) {
            return new PointD(pt.X - sz.Width, pt.Y - sz.Height);
        }
        public static PointD Subtract(PointD pt, SizeF sz) {
            return new PointD(pt.X - sz.Width, pt.Y - sz.Height);
        }
        public static PointD Subtract(PointD pt, Size sz) {
            return new PointD(pt.X - sz.Width, pt.Y - sz.Height);
        }

        public override bool Equals(object obj) {
            if (obj is PointD) {
                return (X == ((PointD) obj).X && Y == ((PointD) obj).Y) ? true : false;
            }
            if (obj is PointF) {
                return ((float) X == ((PointF) obj).X && (float) Y == ((PointF) obj).Y) ? true : false;
            }
            if (obj is Point) {
                return (X == ((Point) obj).X && Y == ((Point) obj).Y) ? true : false;
            }

            return base.Equals(obj);
        }
        public override int GetHashCode() {
            var hashCode = 1861411795;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            return hashCode;
        }
        public override string ToString() {
            return GetType().AssemblyQualifiedName + "{X=" + X + ", Y=" + Y + "}";
        }

        public static PointD operator +(PointD pt, SizeD sz) {
            return Add(pt, sz);
        }
        public static PointD operator +(PointD pt, SizeF sz) {
            return Add(pt, sz);
        }
        public static PointD operator +(PointD pt, Size sz) {
            return Add(pt, sz);
        }
        public static PointD operator -(PointD pt, SizeD sz) {
            return Subtract(pt, sz);
        }
        public static PointD operator -(PointD pt, SizeF sz) {
            return Subtract(pt, sz);
        }
        public static PointD operator -(PointD pt, Size sz) {
            return Subtract(pt, sz);
        }
        public static bool operator ==(PointD left, PointD right) {
            return left.Equals(right);
        }
        public static bool operator !=(PointD left, PointD right) {
            return !left.Equals(right);
        }

        //private

    }
}
