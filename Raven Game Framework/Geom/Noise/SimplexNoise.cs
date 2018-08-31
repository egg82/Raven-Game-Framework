using Raven.Utils;
using System;

namespace Raven.Geom.Noise {
    // Lifted a lot of this from https://github.com/WardBenjamin/SimplexNoise
    public class SimplexNoise : INoise {
        //vars
        private double scale = 1.0d;
        private volatile int seed = 0;

        private volatile byte[] perm = null;

        private static readonly double F2 = 0.36602540378d;
        private static readonly double G2 = 0.2113248654d;

        //constructor
        public SimplexNoise(int seed = 0) {
            this.seed = seed;
            perm = SimplexUtil.GetPerm(seed);
        }

        //public
        public int Seed {
            get {
                return seed;
            }
            set {
                if (value == seed) {
                    return;
                }

                seed = value;
                perm = SimplexUtil.GetPerm(seed);
            }
        }
        public double Scale {
            get {
                return scale;
            }
            set {
                if (value == scale) {
                    return;
                }
                if (double.IsNaN(value) || double.IsInfinity(value)) {
                    throw new NotFiniteNumberException(value);
                }
                if (value < 0.0d) {
                    throw new Exception("value must be positive or zero.");
                }

                scale = value;
            }
        }

        public double[] CalculateAll(int x, int width, uint octave = 0) {
            double[] values = new double[width];
            for (int i = 0; i < width; i++) {
                values[i] = (octave > 0) ? Fbm(i + x, octave) : Generate((i + x) * scale) * 128.0d + 128.0d;
            }
            return values;
        }
        public double[,] CalculateAll(int x, int y, int width, int height, uint octave = 0) {
            double[,] values = new double[width, height];
            for (int i = 0; i < width; i++) {
                for (int j = 0; j < height; j++) {
                    values[i, j] = (octave > 0) ? Fbm(i + x, j + y, octave) : Generate((i + x) * scale, (j + y) * scale) * 128.0d + 128.0d;
                }
            }
            return values;
        }

        public double Calculate(int x, uint octave = 0) {
            return (octave > 0) ? Fbm(x, octave) : Generate(x * scale) * 128.0d + 128.0d;
        }
        public double Calculate(int x, int y, uint octave = 0) {
            return (octave > 0) ? Fbm(x, y, octave) : Generate(x * scale, y * scale) * 128.0d + 128.0d;
        }

        //private
        private double Fbm(int x, uint octave) {
            double f = 0.0d;
            double w = 0.5d;
            for (int i = 0; i < octave; i++) {
                f += w * Generate(x * scale) * 128.0d + 128.0d;
                x *= 2;
                w *= 0.5d;
            }
            return f;
        }
        private double Fbm(int x, int y, uint octave) {
            double f = 0.0d;
            double w = 0.5d;
            for (int i = 0; i < octave; i++) {
                f += w * Generate(x * scale, y * scale) * 128.0d + 128.0d;
                x *= 2;
                y *= 2;
                w *= 0.5d;
            }
            return f;
        }

        private double Generate(double x) {
            int i0 = SimplexUtil.Floor(x);
            int i1 = i0 + 1;
            double x0 = x - i0;
            double x1 = x0 - 1.0d;

            double n0;
            double n1;

            double t0 = 1.0d - x0 * x0;
            t0 *= t0;
            n0 = t0 * t0 * SimplexUtil.Grad(perm[i0 & 0xff], x0);

            double t1 = 1.0d - x1 * x1;
            t1 *= t1;
            n1 = t1 * t1 * SimplexUtil.Grad(perm[i1 & 0xff], x1);
            return 0.395d * (n0 + n1);
        }
        private double Generate(double x, double y) {
            double n0;
            double n1;
            double n2;

            double s = (x + y) * F2;
            double xs = x + s;
            double ys = y + s;
            int i = SimplexUtil.Floor(xs);
            int j = SimplexUtil.Floor(ys);

            double t = (i + j) * G2;
            double X0 = i - t;
            double Y0 = j - t;
            double x0 = x - X0;
            double y0 = y - Y0;

            int i1;
            int j1;

            if (x0 > y0) {
                i1 = 1;
                j1 = 0;
            } else {
                i1 = 0;
                j1 = 1;
            }

            double x1 = x0 - i1 + G2;
            double y1 = y0 - j1 + G2;
            double x2 = x0 - 1.0d + 2.0d * G2;
            double y2 = y0 - 1.0d + 2.0d * G2;

            int ii = SimplexUtil.Mod(i, 256);
            int jj = SimplexUtil.Mod(j, 256);

            double t0 = 0.5d - x0 * x0 - y0 * y0;
            if (t0 < 0.0d) {
                n0 = 0.0d;
            } else {
                t0 *= t0;
                n0 = t0 * t0 * SimplexUtil.Grad(perm[ii + perm[jj]], x0, y0);
            }

            double t1 = 0.5d - x1 * x1 - y1 * y1;
            if (t1 < 0.0d) {
                n1 = 0.0d;
            } else {
                t1 *= t1;
                n1 = t1 * t1 * SimplexUtil.Grad(perm[ii + i1 + perm[jj + j1]], x1, y1);
            }

            double t2 = 0.5d - x2 * x2 - y2 * y2;
            if (t2 < 0.0d) {
                n2 = 0.0d;
            } else {
                t2 *= t2;
                n2 = t2 * t2 * SimplexUtil.Grad(perm[ii + 1 + perm[jj + 1]], x2, y2);
            }

            return 40.0d * (n0 + n1 + n2);
        }
    }
}
