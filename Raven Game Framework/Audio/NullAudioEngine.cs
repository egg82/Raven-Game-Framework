using Raven.Audio.Core;
using Raven.Events;
using Raven.Overrides;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

namespace Raven.Audio {
    public class NullAudioEngine : AbstractAudioEngine {
        //vars
        public override event EventHandler<ExceptionEventArgs> Error;

        private ConcurrentDictionary<string, NullAudio> audio = new ConcurrentDictionary<string, NullAudio>();
        private readonly object outLock = new object();

        //constructor
        public NullAudioEngine() : base() {
            
        }

        //public
        public override int OutputDevice {
            get {
                return base.OutputDevice;
            }
            set {
                lock (outLock) {
                    if (value == base.OutputDevice) {
                        return;
                    }

                    base.OutputDevice = value;
                    foreach (KeyValuePair<string, NullAudio> kvp in audio) {
                        long position = kvp.Value.PositionInBytes;
                        bool playing = kvp.Value.Playing;
                        kvp.Value.Init(base.OutputDevice);
                        kvp.Value.PositionInBytes = position;
                        if (playing) {
                            kvp.Value.Play(kvp.Value.Repeat);
                        }
                    }
                }
            }
        }

        public override IAudio Add(string name, AudioType type, AudioFormat format, byte[] data) {
            if (name == null) {
                throw new ArgumentNullException("name");
            }
            if (data == null) {
                throw new ArgumentNullException("data");
            }

            return audio.AddIfAbsent(name, new NullAudio(type, format, data, base.OutputDevice));
        }
        public override IAudio Remove(string name) {
            if (name == null) {
                throw new ArgumentNullException("name");
            }

            NullAudio retVal = null;
            audio.TryRemove(name, out retVal);
            return retVal;
        }
        public override IAudio Get(string name) {
            if (name == null) {
                throw new ArgumentNullException("name");
            }

            NullAudio retVal = null;
            audio.TryGetValue(name, out retVal);
            return retVal;
        }
        public override int Count {
            get {
                return 0;
            }
        }

        public override void StartRecording(ref Stream stream) {
            if (stream == null) {
                throw new ArgumentNullException("stream");
            }
        }
        public override void StopRecording(ref Stream stream) {
            if (stream == null) {
                throw new ArgumentNullException("stream");
            }
        }

        //private

    }
}
