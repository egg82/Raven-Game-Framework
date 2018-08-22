using Raven.Events;
using Raven.Utils;
using System;
using System.Collections.Immutable;
using System.Threading.Atomics;

namespace Raven.Audio.Core {
    public abstract class AbstractAudio : IAudio {
        //vars
        public abstract event EventHandler<ExceptionEventArgs> Error;

        private readonly AudioType type = AudioType.None;
        private readonly AudioFormat format = AudioFormat.None;
        private volatile bool repeat = false;

        protected readonly byte[] data = null;
        private double volume = 1.0d;
        private double pan = 0.0d;
        protected int device = 0;

        protected AtomicBoolean playing = new AtomicBoolean(false);

        //constructor
        internal AbstractAudio(AudioType type, AudioFormat format, byte[] data, int device) {
            if (data == null) {
                throw new ArgumentNullException("data");
            }

            this.type = type;
            this.format = format;
            this.data = data;
            this.device = device;
        }

        //public
        public AudioType Type {
            get {
                return type;
            }
        }
        public AudioFormat Format {
            get {
                return format;
            }
        }

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
        public int Device {
            get {
                return device;
            }
        }
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
        public bool Repeat {
            get {
                return repeat;
            }
            set {
                this.repeat = value;
            }
        }

        public abstract void Play(bool repeat = false);
        public abstract void Pause();
        public abstract void Stop();

        public abstract long PositionInBytes { get; set; }
        public abstract TimeSpan PositionInTime { get; set; }
        public abstract long LengthInBytes { get; }
        public abstract TimeSpan LengthInTime { get; }
        
        //private

    }
}
