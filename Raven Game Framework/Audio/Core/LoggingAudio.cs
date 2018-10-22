using log4net;
using NAudio.Wave;
using System;
using System.Reflection;

namespace Raven.Audio.Core {
    public class LoggingAudio : Audio {
        // vars
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string name = null;

        // constructor
        internal LoggingAudio(string name, AudioType type, AudioFormat format, byte[] data, int device) : base(type, format, data, device) {
            log.Debug("Created new audio \"" + name + "\" with type=" + type + ", format=" + format + ", length=" + data.LongLength);
            this.name = name;
        }

        // public
        public override double Volume {
            get {
                return base.Volume;
            }
            set {
                log.Debug("Attempting to set volume of audio \"" + name + "\" to " + value);
                base.Volume = value;
            }
        }
        public override double Pan {
            get {
                return base.Pan;
            }
            set {
                log.Debug("Attempting to set pan of audio \"" + name + "\" to " + value);
                base.Pan = value;
            }
        }

        public override void Play(bool repeat = false) {
            log.Debug("Attempting to play audio \"" + name + "\"");
            base.Play(repeat);
        }
        public override void Pause() {
            log.Debug("Attempting to pause audio \"" + name + "\"");
            base.Pause();
        }
        public override void Stop() {
            log.Debug("Attempting to stop audio \"" + name + "\"");
            base.Stop();
        }

        public override long PositionInBytes {
            get {
                return base.PositionInBytes;
            }
            set {
                log.Debug("Attempting to set position (bytes) of audio \"" + name + "\" to " + value);
                base.PositionInBytes = value;
            }
        }
        public override TimeSpan PositionInTime {
            get {
                return base.PositionInTime;
            }
            set {
                log.Debug("Attempting to set position (time) of audio \"" + name + "\" to " + value);
                base.PositionInTime = value;
            }
        }

        // private
        protected override void OnPlaybackComplete(object sender, StoppedEventArgs e) {
            if (e.Exception != null) {
                log.Error("Audio \"" + name + "\" threw exception.", e.Exception);
            } else {
                log.Debug("Audio \"" + name + "\" finished, replaying if needed.");
            }
            base.OnPlaybackComplete(sender, e);
        }
    }
}
