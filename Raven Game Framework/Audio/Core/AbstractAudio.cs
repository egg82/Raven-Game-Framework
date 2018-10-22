using Raven.Events;
using Raven.Utils;
using System;
using System.Collections.Immutable;
using System.Threading.Atomics;

namespace Raven.Audio.Core {
    public abstract class AbstractAudio : IAudio {
        // events
        public abstract event EventHandler<ExceptionEventArgs> Error;

        // vars
        protected readonly byte[] data = null;
        private double volume = 1.0d;
        private double pan = 0.0d;

        protected AtomicBoolean playing = new AtomicBoolean(false);

        // constructor
        protected AbstractAudio(AudioType type, AudioFormat format, byte[] data, int device) {
            if (data == null) {
                throw new ArgumentNullException("data");
            }

            Type = type;
            Format = format;
            this.data = data;
            Device = device;
        }

        // public
        public AudioType Type { get; private set; }
        public AudioFormat Format { get; private set; }
        public bool Repeat { get; set; }

        public virtual double Volume {
            get {
                return volume;
            }
            set {
                if (value == volume || double.IsNaN(value) || double.IsInfinity(value)) {
                    return;
                }
                volume = MathUtil.Clamp(0.0d, 1.0d, value);
            }
        }
        public virtual double Pan {
            get {
                return pan;
            }
            set {
                if (value == pan || double.IsNaN(value) || double.IsInfinity(value)) {
                    return;
                }
                pan = MathUtil.Clamp(-1.0d, 1.0d, value);
            }
        }
        public virtual int Device { get; internal set; }
        public ImmutableArray<byte> Data {
            get {
                return ImmutableArray.ToImmutableArray(data);
            }
        }
        public bool Playing {
            get {
                return playing;
            }
        }

        public abstract void Play(bool repeat = false);
        public abstract void Pause();
        public abstract void Stop();

        public abstract long PositionInBytes { get; set; }
        public abstract TimeSpan PositionInTime { get; set; }

        public abstract long LengthInBytes { get; }
        public abstract TimeSpan LengthInTime { get; }
        
        // private

    }
}
