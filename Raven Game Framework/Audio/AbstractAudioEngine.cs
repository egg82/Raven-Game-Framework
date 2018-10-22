using NAudio.Wave;
using Raven.Audio.Core;
using Raven.Events;
using System;
using System.Collections.Immutable;
using System.IO;
using System.Threading;

namespace Raven.Audio {
    public abstract class AbstractAudioEngine : IAudioEngine {
        // events
        public abstract event EventHandler<ExceptionEventArgs> Error;

        // vars
        private int outDevice = 0;
        private readonly string[] outputDevices = new string[WaveOut.DeviceCount];
        private int inDevice = 0;
        private readonly string[] inputDevices = new string[WaveIn.DeviceCount];

        // constructor
        protected AbstractAudioEngine() {
            for (int i = 0; i < WaveIn.DeviceCount; i++) {
                inputDevices[i] = WaveIn.GetCapabilities(i).ProductName;
            }
            for (int i = 0; i < WaveOut.DeviceCount; i++) {
                outputDevices[i] = WaveOut.GetCapabilities(i).ProductName;
            }
        }

        // public
        public virtual int InputDevice {
            get {
                return inDevice;
            }
            set {
                if (value < 0 || value > WaveIn.DeviceCount) {
                    throw new ArgumentOutOfRangeException("value");
                }

                Interlocked.Exchange(ref inDevice, value);
            }
        }
        public string InputDeviceName {
            get {
                return WaveIn.GetCapabilities(inDevice).ProductName;
            }
        }
        public ImmutableArray<string> InputDeviceNames {
            get {
                return ImmutableArray.ToImmutableArray(inputDevices);
            }
        }

        public virtual int OutputDevice {
            get {
                return outDevice;
            }
            set {
                if (value < 0 || value > WaveOut.DeviceCount) {
                    throw new ArgumentOutOfRangeException("value");
                }
                
                Interlocked.Exchange(ref outDevice, value);
            }
        }
        public string OutputDeviceName {
            get {
                return WaveOut.GetCapabilities(outDevice).ProductName;
            }
        }
        public ImmutableArray<string> OutputDeviceNames {
            get {
                return ImmutableArray.ToImmutableArray(outputDevices);
            }
        }

        public abstract IAudio Add(string name, AudioType type, AudioFormat format, byte[] data);
        public abstract IAudio Remove(string name);
        public abstract IAudio Get(string name);
        public abstract int Count { get; }

        public abstract void StartRecording(ref Stream stream);
        public abstract void StopRecording(ref Stream stream);

        // private

    }
}
