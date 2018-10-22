using Raven.Audio.Core;
using Raven.Events;
using System;
using System.Collections.Immutable;
using System.IO;

namespace Raven.Audio {
    public interface IAudioEngine {
        event EventHandler<ExceptionEventArgs> Error;
        
        int InputDevice { get; set; }
        string InputDeviceName { get; }
        ImmutableArray<string> InputDeviceNames { get; }

        int OutputDevice { get; set; }
        string OutputDeviceName { get; }
        ImmutableArray<string> OutputDeviceNames { get; }

        IAudio Add(string name, AudioType type, AudioFormat format, byte[] data);
        IAudio Remove(string name);
        IAudio Get(string name);
        int Count { get; }

        void StartRecording(ref Stream stream);
        void StopRecording(ref Stream stream);
    }
}
