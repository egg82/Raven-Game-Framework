using System;

namespace Raven.Utils {
    public class MathUtil {
        // vars
        private static Random random = new Random();

        // constructor
        private MathUtil() {

        }

        // public
        public static double Random(double min, double max) {
            return random.NextDouble() * (max - min) + min;
        }
        public static int FairRoundedRandom(int min, int max) {
            int num;
            max++;

            do {
                num = (int) Math.Floor(random.NextDouble() * (max - min) + min);
            } while (num > max - 1);

            return num;
        }

        public static decimal Clamp(decimal min, decimal max, decimal val) {
            return Math.Min(max, Math.Max(min, val));
        }
        public static double Clamp(double min, double max, double val) {
            return Math.Min(max, Math.Max(min, val));
        }
        public static float Clamp(float min, float max, float val) {
            return Math.Min(max, Math.Max(min, val));
        }
        public static int Clamp(int min, int max, int val) {
            return Math.Min(max, Math.Max(min, val));
        }
        public static uint Clamp(uint min, uint max, uint val) {
            return Math.Min(max, Math.Max(min, val));
        }
        public static short Clamp(short min, short max, short val) {
            return Math.Min(max, Math.Max(min, val));
        }
        public static ushort Clamp(ushort min, ushort max, ushort val) {
            return Math.Min(max, Math.Max(min, val));
        }
        public static long Clamp(long min, long max, long val) {
            return Math.Min(max, Math.Max(min, val));
        }
        public static ulong Clamp(ulong min, ulong max, ulong val) {
            return Math.Min(max, Math.Max(min, val));
        }
        public static byte Clamp(byte min, byte max, byte val) {
            return Math.Min(max, Math.Max(min, val));
        }
        public static sbyte Clamp(sbyte min, sbyte max, sbyte val) {
            return Math.Min(max, Math.Max(min, val));
        }

        public static uint UpperPowerOfTwo(uint v) {
            if (v < 0) {
                v = 0;
            }

            v--;
            v |= v >> 1;
            v |= v >> 2;
            v |= v >> 4;
            v |= v >> 8;
            v |= v >> 16;
            v++;

            return v;
        }
        public static ulong UpperPowerOfTwo(ulong v) {
            if (v < 0L) {
                v = 0L;
            }

            v--;
            v |= v >> 1;
            v |= v >> 2;
            v |= v >> 4;
            v |= v >> 8;
            v |= v >> 16;
            v |= v >> 32;
            v++;

            return v;
        }

        // private

    }
}
