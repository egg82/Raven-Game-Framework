using NAudio.Wave;
using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Raven.Audio.Exceptions {
    public abstract class AudioException : Exception {
        // vars

        // constructor
        protected AudioException(WaveOutEvent waveOut, string message, Exception innerException = null) : base(message, innerException) {
            WaveOut = waveOut;
        }
        protected AudioException(WaveInEvent waveIn, string message, Exception innerException = null) : base(message, innerException) {
            WaveIn = waveIn;
        }

        protected AudioException(SerializationInfo info, StreamingContext context) : base(info, context) {
            WaveOut = (WaveOutEvent) info.GetValue("WaveOut", typeof(WaveOutEvent));
            WaveIn = (WaveInEvent) info.GetValue("WaveIn", typeof(WaveInEvent));
        }

        // public
        public WaveOutEvent WaveOut { get; private set; }
        public WaveInEvent WaveIn { get; private set; }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        override public void GetObjectData(SerializationInfo info, StreamingContext context) {
            if (info == null) {
                throw new ArgumentNullException("info");
            }
            info.AddValue("WaveOut", WaveOut);
            info.AddValue("WaveIn", WaveIn);

            base.GetObjectData(info, context);
        }

        // private

    }
}
