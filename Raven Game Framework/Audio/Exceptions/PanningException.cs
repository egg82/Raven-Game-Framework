using NAudio.Wave;
using System;
using System.Runtime.Serialization;

namespace Raven.Audio.Exceptions {
    [Serializable]
    public class PanningException : AudioException {
        // vars

        // constructor
        public PanningException(WaveOutEvent waveOut) : base(waveOut, "Audio is not mono and cannot be panned.") {

        }
        protected PanningException(SerializationInfo info, StreamingContext context) : base(info, context) {

        }

        // public

        // private

    }
}
