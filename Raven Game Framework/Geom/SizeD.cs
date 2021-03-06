﻿using System;
using System.Drawing;

namespace Raven.Geom {
    public class SizeD {
        // vars
        public static readonly SizeD Empty = new SizeD(0.0d, 0.0d);

        // constructor
        public SizeD() : this(0.0d, 0.0d) {

        }
        public SizeD(SizeD size) {
            if (size == null) {
                throw new ArgumentNullException("size");
            }

            Width = size.Width;
            Height = size.Height;
        }
        public SizeD(PointD point) {
            if (point == null) {
                throw new ArgumentNullException("point");
            }

            Width = point.X;
            Height = point.Y;
        }
        public SizeD(double width, double height) {
            Width = width;
            Height = height;
        }

        // public
        public double Width { get; set; }
        public double Height { get; set; }
        public bool IsEmpty {
            get {
                return Width == 0.0d && Height == 0.0d;
            }
        }

        public static SizeD Add(SizeD sz1, SizeD sz2) {
            return new SizeD(sz1.Width + sz2.Width, sz1.Height + sz2.Height);
        }
        public static SizeD Subtract(SizeD sz1, SizeD sz2) {
            return new SizeD(sz1.Width - sz2.Width, sz1.Height - sz2.Height);
        }
        public override bool Equals(object obj) {
            if (ReferenceEquals(this, obj)) {
                return true;
            }

            if (obj is SizeD d) {
                return Width == d.Width && Height == d.Height;
            }
            if (obj is SizeF f) {
                return (float) Width == f.Width && (float) Height == f.Height;
            }
            if (obj is Size i) {
                return Width == i.Width && Height == i.Height;
            }

            return false;
        }
        public override int GetHashCode() {
            var hashCode = 859600377;
            hashCode = hashCode * -1521134295 + Width.GetHashCode();
            hashCode = hashCode * -1521134295 + Height.GetHashCode();
            return hashCode;
        }
        public override string ToString() {
            return GetType().AssemblyQualifiedName + "{Width=" + Width + ", Height=" + Height + "}";
        }

        public PointD ToPointD() {
            return new PointD(Width, Height);
        }
        public PointF ToPointF() {
            return new PointF((float) Width, (float) Height);
        }
        public SizeF ToSizeF() {
            return new SizeF((float) Width, (float) Height);
        }
        public Size ToSize() {
            return new Size((int) Math.Floor(Width), (int) Math.Floor(Height));
        }

        public static SizeD operator +(SizeD sz1, SizeD sz2) {
            return Add(sz1, sz2);
        }
        public static SizeD operator -(SizeD sz1, SizeD sz2) {
            return Subtract(sz1, sz2);
        }
        public static bool operator ==(SizeD left, SizeD right) {
            return left.Equals(right);
        }
        public static bool operator !=(SizeD left, SizeD right) {
            return !left.Equals(right);
        }
        public static explicit operator PointD(SizeD size) {
            return new PointD(size.Width, size.Height);
        }

        // private

    }
}
