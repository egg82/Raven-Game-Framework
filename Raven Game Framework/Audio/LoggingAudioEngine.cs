using log4net;
using NAudio.Wave;
using Raven.Audio.Core;
using Raven.Events;
using System.IO;
using System.Reflection;

namespace Raven.Audio {
    public class LoggingAudioEngine : AudioEngine {
        //vars
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        //constructor
        public LoggingAudioEngine() : base() {
            log.Debug("Audio engine enabled.");
        }

        //public
        public override int InputDevice {
            get {
                return base.InputDevice;
            }
            set {
                log.Debug("Attempting to set input device to " + value);
                base.InputDevice = value;
            }
        }
        public override int OutputDevice {
            get {
                return base.OutputDevice;
            }
            set {
                log.Debug("Attempting to set output device to " + value);
                base.OutputDevice = value;
            }
        }

        public override IAudio Add(string name, AudioType type, AudioFormat format, byte[] data) {
            log.Debug("Attempting to add audio \"" + name + "\" with type=" + type + ", format=" + format + ", length=" + data.LongLength);
            return Add(name, new Core.LoggingAudio(name, type, format, data, base.OutputDevice));
        }
        public override IAudio Remove(string name) {
            log.Debug("Attempting to remove audio \"" + name + "\"");
            return base.Remove(name);
        }

        public override void StartRecording(ref Stream stream) {
            log.Debug("Attempting to record input to stream " + stream);
            base.StartRecording(ref stream);
        }
        public override void StopRecording(ref Stream stream) {
            log.Debug("Attempting to stop recording stream " + stream);
            base.StopRecording(ref stream);
        }

        //private
        protected override void OnError(object sender, ExceptionEventArgs e) {
            log.Error("Bubbling exception.", e.Exception);
            base.OnError(sender, e);
        }
        protected override void OnRecordingComplete(object sender, StoppedEventArgs e) {
            if (e.Exception != null) {
                log.Error("Input stream threw exception.", e.Exception);
            }
            base.OnRecordingComplete(sender, e);
        }
        protected override void OnInputData(object sender, WaveInEventArgs e) {
            if (inStreams.Count > 0) {
                log.Debug("Input stream has data with length=" + e.BytesRecorded);
            }
            base.OnInputData(sender, e);
        }
    }
}
