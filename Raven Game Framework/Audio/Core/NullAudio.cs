using System;
using Raven.Events;

namespace Raven.Audio.Core {
    public class NullAudio : AbstractAudio {
        // events
        public override event EventHandler<ExceptionEventArgs> Error;

        // vars

        // constructor
        internal NullAudio(AudioType type, AudioFormat format, byte[] data, int device) : base(type, format, data, device) {
            PositionInBytes = 0L;
            PositionInTime = TimeSpan.Zero;
        }
        
        public override void Play(bool repeat = false) {
            playing.Value = true;
        }
        public override void Pause() {
            playing.Value = false;
        }
        public override void Stop() {
            playing.Value = false;
        }

        public override long PositionInBytes { get; set; }
        public override TimeSpan PositionInTime { get; set; }

        public override long LengthInBytes {
            get {
                return data.LongLength;
            }
        }
        public override TimeSpan LengthInTime {
            get {
                return TimeSpan.Zero;
            }
        }

        // private

    }
}
