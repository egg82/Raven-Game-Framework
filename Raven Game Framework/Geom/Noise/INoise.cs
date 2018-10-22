namespace Raven.Geom.Noise {
    public interface INoise {
        int Seed { get; set; }
        double Scale { get; set; }

        double[] CalculateAll(int x, int width, uint octave = 0);
        double[,] CalculateAll(int x, int y, int width, int height, uint octave = 0);

        double Calculate(int x, uint octave = 0);
        double Calculate(int x, int y, uint octave = 0);
    }
}
