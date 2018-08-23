using JoshuaKearney.Collections;
using log4net;
using Raven.Audio;
using Raven.Audio.Core;
using Raven.Core;
using Raven.Display;
using Raven.Input;
using Raven.Input.Enums;
using Raven.Patterns;
using Raven.Utils;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using static SFML.Window.Keyboard;

namespace Test {
    class Program {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static volatile bool updating = true;

        private static ConcurrentSet<Window> windows = new ConcurrentSet<Window>();

        static void Main(string[] args) {
            Console.CancelKeyPress += OnCancel;

            UpdateDeltaLoop updateLoop = new UpdateDeltaLoop(delegate(double deltaTime) {
                ServiceLocator.GetService<IInputEngine>()?.UpdateControllers();
                foreach (Window w in windows) {
                    w.UpdateWindow();
                    w.UpdateStates(deltaTime);
                }
            }, 120.0d);
            UpdateDeltaLoop drawLoop = new UpdateDeltaLoop(delegate() {
                foreach (Window w in windows) {
                    w.DrawGraphics();
                }
            }, 60.0d);

            ThreadPool.QueueUserWorkItem(delegate(object state) {
                //Audio();
                //Input();
                Graphics();

                ThreadUtil.PreventSystemSleep();

                foreach (Window w in windows) {
                    w.Closed += OnWindowClose;
                }

                /*do {
                    ServiceLocator.GetService<IInputEngine>()?.UpdateControllers();
                    foreach (Window w in windows) {
                        w.UpdateWindow();
                        w.UpdateStates(1.0d);
                    }
                } while (updating);*/
                updateLoop.Start();
            });
            ThreadPool.QueueUserWorkItem(delegate(object state) {
                /*do {
                    foreach (Window w in windows) {
                        w.DrawGraphics();
                    }
                } while (updating);*/
                drawLoop.Start();
            });

            do {
                Thread.Sleep(50);
            } while (updating);
            updateLoop.Stop();
            drawLoop.Stop();
            
            Console.WriteLine("Execution stopped.");
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

            Window window = new Window(800, 600, "Input", SFML.Window.Styles.Default, false);
            windows.Add(window);
            log.Info("Created window \"" + window.Title + "\"");

            IInputEngine inputEngine = ServiceLocator.GetService<IInputEngine>();
            log.Info("Got service " + inputEngine);

            //inputEngine.Controllers.Vibrate(0, 1.0d, 1.0d);

            inputEngine.AddWindow(window);
            log.Info("Added window \"" + window.Title + "\"");
        }
        private static void Graphics() {
            Window window = new Window(800, 600, "Graphics", SFML.Window.Styles.Default, false);
            windows.Add(window);
            log.Info("Created window \"" + window.Title + "\"");
        }

        private static void OnCancel(object sender, ConsoleCancelEventArgs e) {
            e.Cancel = true;
            updating = false;
        }
        private static void OnWindowClose(object sender, EventArgs e) {
            Window w = (Window) sender;
            windows.Remove(w);
            w.Close();
        }
    }
}
