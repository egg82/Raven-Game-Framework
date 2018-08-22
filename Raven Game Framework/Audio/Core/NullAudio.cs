using System;
using Raven.Events;

namespace Raven.Audio.Core {
    public class NullAudio : AbstractAudio {
        //vars
        public override event EventHandler<ExceptionEventArgs> Error;

        //constructor
        internal NullAudio(AudioType type, AudioFormat format, byte[] data, int device) : base(type, format, data, device) {
            
        }
        
        public override void Play(bool repeat = false) {
            base.playing.Value = true;
        }
        public override void Pause() {
            base.playing.Value = false;
        }
        public override void Stop() {
            base.playing.Value = false;
        }

        public override long PositionInBytes {
            get {
                return 0L;
            }
            set {
                
            }
        }
        public override TimeSpan PositionInTime {
            get {
                return TimeSpan.Zero;
            }
            set {
                
            }
        }
        public override long LengthInBytes {
            get {
                return data.LongLength;
            }
        }
        public override TimeSpan LengthInTime {
            get {
                return TimeSpan.MaxValue;
            }
        }

        //private
        internal virtual void Init(int device) {
            playing.Value = false;
            this.device = device;
        }
    }
}
