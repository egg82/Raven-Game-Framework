using Raven.Events;
using System;
using System.Collections.Immutable;

namespace Raven.Audio.Core {
    public interface IAudio {
        //functions
        event EventHandler<ExceptionEventArgs> Error;

        AudioType Type { get; }
        AudioFormat Format { get; }
        double Volume { get; set; }
        double Pan { get; set; }
        int Device { get; }
        ImmutableArray<byte> Data { get; }
        bool Playing { get; }
        bool Repeat { get; set; }

        void Play(bool repeat = false);
        void Pause();
        void Stop();

        long PositionInBytes { get; set; }
        TimeSpan PositionInTime { get; set; }
        long LengthInBytes { get; }
        TimeSpan LengthInTime { get; }
    }

    public enum AudioType {
        Ambient,
        Music,
        SFX,
        UI,
        Voice,
        Other,
        None
    }
    public enum AudioFormat {
        AIFF,
        WAV,
        MP3,
        Vorbis,
        None
    }
}
