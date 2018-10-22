using NAudio.Vorbis;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Raven.Audio.Exceptions;
using Raven.Events;
using Raven.Utils;
using System;
using System.IO;

namespace Raven.Audio.Core {
    public class Audio : AbstractAudio {
        // events
        public override event EventHandler<ExceptionEventArgs> Error = null;

        // vars
        private readonly MemoryStream byteStream = null;
        private readonly WaveStream waveStream = null;
        private readonly VolumeSampleProvider volumeProvider = null;
        private readonly PanningSampleProvider panningProvider = null;
        private readonly WaveOutEvent waveOut = new WaveOutEvent();

        // constructor
        internal Audio(AudioType type, AudioFormat format, byte[] data, int device) : base(type, format, data, device) {
            playing.Value = false;
            
            waveOut.DeviceNumber = device;
            waveOut.PlaybackStopped += OnPlaybackComplete;

            byteStream = new MemoryStream(data);
            if (Format == AudioFormat.AIFF) {
                waveStream = new AiffFileReader(byteStream);
            } else if (Format == AudioFormat.WAV) {
                waveStream = new WaveFileReader(byteStream);
            } else if (Format == AudioFormat.MP3) {
                waveStream = new Mp3FileReader(byteStream);
            } else if (Format == AudioFormat.Vorbis) {
                waveStream = new VorbisWaveReader(byteStream);
            }
            volumeProvider = new VolumeSampleProvider(waveStream.ToSampleProvider()) {
                Volume = (float) Volume
            };

            if (waveStream.WaveFormat.Channels == 1) {
                panningProvider = new PanningSampleProvider(volumeProvider) {
                    Pan = (float) Pan
                };
                waveOut.Init(panningProvider);
            } else {
                waveOut.Init(volumeProvider);
            }
        }

        // public
        public override double Volume {
            get {
                return base.Volume;
            }
            set {
                if (base.Volume == value) {
                    return;
                }

                base.Volume = value;
                volumeProvider.Volume = (float) base.Volume;
            }
        }
        public override double Pan {
            get {
                return base.Pan;
            }
            set {
                if (panningProvider == null) {
                    throw new PanningException(waveOut);
                }
                if (base.Pan == value) {
                    return;
                }

                base.Pan = value;
                panningProvider.Pan = (float) base.Pan;
            }
        }
        public override int Device {
            get {
                return base.Device;
            }
            internal set {
                waveOut.DeviceNumber = value;
                base.Device = value;
            }
        }

        public override void Play(bool repeat = false) {
            Repeat = repeat;

            if (playing.CompareExchange(true, false)) {
                return;
            }
            
            waveOut.Play();
        }
        public override void Pause() {
            if (!playing.CompareExchange(false, true)) {
                return;
            }

            waveOut.Pause();
        }
        public override void Stop() {
            playing.Value = false;
            waveOut.Stop();
        }

        public override long PositionInBytes {
            get {
                return waveStream.Position;
            }
            set {
                waveStream.Position = MathUtil.Clamp(0L, data.LongLength, value);
            }
        }
        public override TimeSpan PositionInTime {
            get {
                return waveStream.CurrentTime;
            }
            set {
                waveStream.CurrentTime = value;
            }
        }

        public override long LengthInBytes {
            get {
                return data.LongLength;
            }
        }
        public override TimeSpan LengthInTime {
            get {
                return waveStream.TotalTime;
            }
        }

        // private
        protected virtual void OnPlaybackComplete(object sender, StoppedEventArgs e) {
            if (e.Exception != null) {
                Error?.Invoke(this, new ExceptionEventArgs(e.Exception));
            } else {
                if (Repeat) {
                    waveStream.Position = 0L;
                    waveOut.Play();
                }
            }
        }
    }
}
