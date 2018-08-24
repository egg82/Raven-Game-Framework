using System;
using System.Drawing;

namespace Raven.Geom {
    public class RectD {
        //vars
        public static readonly RectD Empty = new RectD(0.0d, 0.0d, 0.0d, 0.0d);

        //constructor
        public RectD() : this(0.0d, 0.0d, 0.0d, 0.0d) {

        }
        public RectD(PointD pt, SizeD sz) {
            if (pt == null) {
                throw new ArgumentNullException("pt");
            }
            if (sz == null) {
                throw new ArgumentNullException("sz");
            }

            X = pt.X;
            Y = pt.Y;
            Width = sz.Width;
            Height = sz.Height;
        }
        public RectD(Point pt, Size sz) {
            if (pt == null) {
                throw new ArgumentNullException("pt");
            }
            if (sz == null) {
                throw new ArgumentNullException("sz");
            }

            X = pt.X;
            Y = pt.Y;
            Width = sz.Width;
            Height = sz.Height;
        }
        public RectD(PointF pt, SizeF sz) {
            if (pt == null) {
                throw new ArgumentNullException("pt");
            }
            if (sz == null) {
                throw new ArgumentNullException("sz");
            }

            X = pt.X;
            Y = pt.Y;
            Width = sz.Width;
            Height = sz.Height;
        }
        public RectD(double x, double y, double width, double height) {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        //public
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        //private

    }
}
