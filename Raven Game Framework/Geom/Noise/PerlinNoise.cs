using Raven.Utils;
using System;
using System.Threading;

namespace Raven.Geom.Noise {
    public class PerlinNoise : INoise {
        // vars
        private double scale = 1.0d;
        private int seed = 0;

        private byte[] perm = null;

        // constructor
        public PerlinNoise(int seed = 0) {
            Interlocked.Exchange(ref this.seed, seed);
            Interlocked.Exchange(ref perm, PerlinUtil.GetPerm(seed));
        }

        // public
        public int Seed {
            get {
                return seed;
            }
            set {
                if (value == seed) {
                    return;
                }

                Interlocked.Exchange(ref seed, value);
                Interlocked.Exchange(ref perm, PerlinUtil.GetPerm(seed));
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
                    throw new ArgumentOutOfRangeException("value");
                }

                scale = value;
            }
        }

        public double[] CalculateAll(int x, int width, uint octave = 0) {
            double[] values = new double[width];
            for (int i = 0; i < width; i++) {
                values[i] = (octave > 0) ? Fbm(i + x, octave) : Generate((i + x) * scale);
            }
            return values;
        }
        public double[,] CalculateAll(int x, int y, int width, int height, uint octave = 0) {
            double[,] values = new double[width, height];
            for (int i = 0; i < width; i++) {
                for (int j = 0; j < height; j++) {
                    values[i, j] = (octave > 0) ? Fbm(i + x, j + y, octave) : Generate((i + x) * scale, (j + y) * scale);
                }
            }
            return values;
        }

        public double Calculate(int x, uint octave = 0) {
            return (octave > 0) ? Fbm(x, octave) : Generate(x * scale);
        }
        public double Calculate(int x, int y, uint octave = 0) {
            return (octave > 0) ? Fbm(x, y, octave) : Generate(x * scale, y * scale);
        }

        // private
        private double Fbm(int x, uint octave) {
            double f = 0.0d;
            double w = 0.5d;
            for (int i = 0; i < octave; i++) {
                f += w * Generate(x * scale);
                x *= 2;
                w *= 0.5d;
            }
            return f;
        }
        private double Fbm(int x, int y, uint octave) {
            double f = 0.0d;
            double w = 0.5d;
            for (int i = 0; i < octave; i++) {
                f += w * Generate(x * scale, y * scale);
                x *= 2;
                y *= 2;
                w *= 0.5d;
            }
            return f;
        }
        
        private double Generate(double x) {
            int X = ((int) Math.Floor(x)) & 0xff;
            x -= Math.Floor(x);
            double u = PerlinUtil.Fade(x);
            return PerlinUtil.Lerp(u, PerlinUtil.Grad(perm[X], x), PerlinUtil.Grad(perm[X + 1], x - 1)) * 2.0d;
        }
        private double Generate(double x, double y) {
            int X = ((int) Math.Floor(x)) & 0xff;
            int Y = ((int) Math.Floor(y)) & 0xff;
            x -= Math.Floor(x);
            y -= Math.Floor(y);
            double u = PerlinUtil.Fade(x);
            double v = PerlinUtil.Fade(y);
            int A = (perm[X] + Y) & 0xff;
            int B = (perm[X + 1] + Y) & 0xff;
            return PerlinUtil.Lerp(v,
                PerlinUtil.Lerp(u, PerlinUtil.Grad(perm[A], x, y), PerlinUtil.Grad(perm[B], x - 1.0d, y)),
                PerlinUtil.Lerp(u, PerlinUtil.Grad(perm[A + 1], x, y - 1.0d), PerlinUtil.Grad(perm[B + 1], x - 1.0d, y - 1.0d))
            );
        }
    }
}
