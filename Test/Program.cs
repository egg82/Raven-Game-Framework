using JoshuaKearney.Collections;
using log4net;
using Raven.Audio;
using Raven.Audio.Core;
using Raven.Display;
using Raven.Input;
using Raven.Input.Enums;
using Raven.Patterns;
using System;
using System.IO;
using System.Reflection;
using static SFML.Window.Keyboard;

namespace Test {
    class Program {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static volatile bool updating = true;

        private static ConcurrentSet<Window> windows = new ConcurrentSet<Window>();

        static void Main(string[] args) {
            Console.CancelKeyPress += OnCancel;

            //Audio();
            //Input();
            Graphics();

            do {
                ServiceLocator.GetService<IInputEngine>()?.UpdateControllers();
                foreach (Window w in windows) {
                    w.Update();
                    w.Draw();
                }
            } while (updating);
            Console.WriteLine("Execution stopped.");
            Console.ReadLine();
        }

        private static void Audio() {
            ServiceLocator.ProvideService(typeof(LoggingAudioEngine), false);
            log.Info("Provided service");

            IAudioEngine audioEngine = ServiceLocator.GetService<IAudioEngine>();
            log.Info("Got service " + audioEngine);

            IAudio audio = audioEngine.Add("voice", AudioType.Voice, AudioFormat.MP3, File.ReadAllBytes(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar + "audio" + Path.DirectorySeparatorChar + "voice.mp3"));
            log.Info("Got audio " + audio);

            audio.Volume = 0.15d;
            audio.Play(true);
            log.Info("Playing audio");

            audio = audioEngine.Add("music", AudioType.Music, AudioFormat.MP3, File.ReadAllBytes(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar + "audio" + Path.DirectorySeparatorChar + "music.mp3"));
            log.Info("Got audio2 " + audio);

            audio.Volume = 0.15d;
            audio.Play(false);
            log.Info("Playing audio2");
        }
        private static void Input() {
            ServiceLocator.ProvideService(typeof(LoggingInputEngine), false);
            log.Info("Provided service");

            Window window = new Window("Input");
            windows.Add(window);
            log.Info("Created window \"" + window.Title + "\"");

            IInputEngine inputEngine = ServiceLocator.GetService<IInputEngine>();
            log.Info("Got service " + inputEngine);

            //inputEngine.Controllers.Supported = false;

            inputEngine.Controllers.Vibrate(0, 1.0d, 1.0d);

            inputEngine.AddWindow(window);
            log.Info("Added window \"" + window.Title + "\"");
        }
        private static void Graphics() {
            Window window = new Window("Graphics");
            windows.Add(window);
            log.Info("Created window \"" + window.Title + "\"");
        }

        private static void OnCancel(object sender, ConsoleCancelEventArgs e) {
            e.Cancel = true;
            updating = false;
        }
    }
}
