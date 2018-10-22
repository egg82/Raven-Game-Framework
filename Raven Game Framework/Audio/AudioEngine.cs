using JoshuaKearney.Collections;
using NAudio.Wave;
using Raven.Audio.Core;
using Raven.Events;
using Raven.Overrides;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

namespace Raven.Audio {
    public class AudioEngine : AbstractAudioEngine {
        // events
        public override event EventHandler<ExceptionEventArgs> Error = null;

        // vars
        private readonly ConcurrentDictionary<string, Core.Audio> audio = new ConcurrentDictionary<string, Core.Audio>();
        private readonly object outLock = new object();

        private readonly WaveInEvent waveIn = new WaveInEvent();
        protected ConcurrentSet<Stream> inStreams = new ConcurrentSet<Stream>();
        private readonly object inLock = new object();

        // constructor
        public AudioEngine() : base() {
            waveIn.RecordingStopped += OnRecordingComplete;
            waveIn.DataAvailable += OnInputData;
            waveIn.StartRecording();
        }

        // public
        public override int InputDevice {
            get {
                return base.InputDevice;
            }
            set {
                lock (inLock) {
                    if (value == base.InputDevice) {
                        return;
                    }

                    base.InputDevice = value;
                    waveIn.StopRecording();
                    waveIn.DeviceNumber = base.InputDevice;
                    waveIn.StartRecording();
                }
            }
        }
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
                    foreach (KeyValuePair<string, Core.Audio> kvp in audio) {
                        long position = kvp.Value.PositionInBytes;
                        bool playing = kvp.Value.Playing;
                        kvp.Value.Device = base.OutputDevice;
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

            return Add(name, new Core.Audio(type, format, data, base.OutputDevice));
        }
        public override IAudio Remove(string name) {
            if (name == null) {
                throw new ArgumentNullException("name");
            }

            audio.TryRemove(name, out Core.Audio retVal);
            return retVal;
        }
        public override IAudio Get(string name) {
            if (name == null) {
                throw new ArgumentNullException("name");
            }

            audio.TryGetValue(name, out Core.Audio retVal);
            return retVal;
        }
        public override int Count {
            get {
                return audio.Count;
            }
        }

        public override void StartRecording(ref Stream stream) {
            if (stream == null) {
                throw new ArgumentNullException("stream");
            }
            if (!stream.CanWrite) {
                return;
            }

            inStreams.Add(stream);
        }
        public override void StopRecording(ref Stream stream) {
            if (stream == null) {
                throw new ArgumentNullException("stream");
            }

            inStreams.Remove(stream);
        }

        // private
        protected IAudio Add(string name, Core.Audio audio) {
            IAudio retVal = this.audio.AddIfAbsent(name, audio);
            retVal.Error += OnError;

            return retVal;
        }
        
        protected virtual void OnError(object sender, ExceptionEventArgs e) {
            Error?.Invoke(sender, e);
        }
        protected virtual void OnRecordingComplete(object sender, StoppedEventArgs e) {
            if (e.Exception != null) {
                Error?.Invoke(this, new ExceptionEventArgs(e.Exception));
            }
        }
        protected virtual void OnInputData(object sender, WaveInEventArgs e) {
            foreach (Stream s in inStreams) {
                if (s == null || !s.CanWrite) {
                    inStreams.Remove(s);
                    continue;
                }

                s.Write(e.Buffer, 0, e.BytesRecorded);
            }
        }
    }
}
