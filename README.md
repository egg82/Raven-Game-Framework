# Raven Game Framework
2D game framework written in C# that is designed to be easy to use, but still enforces good practices.
This is based off of the Starling/Sparrow projects. The idea is to make putting graphics on the screen extremely easy, along with handling input, audio, and physics.
Originally this project was called "Egg82LibEnhanced" but I scrapped both the project and the title as both were terrible.

# Usage
More examples available in the "Test" project
### Input (Keyboard, Mouse, DXInput)
The input engine uses events and also stores states at the same time.
This means you can have your input handling be either event-driven or update-driven. The choice is yours!
```CSharp
ServiceLocator.ProvideService(typeof(InputEngine)); // Also available: NullInputEngine & LoggingInputEngine
...
IInputEngine inputEngine = ServiceLocator.GetService<IInputEngine>();
Window window = new Window(800, 600, "Input"); // Window is required for keyboard/mouse since that's what traps the events
inputEngine.AddWindow(window);

...

// Event-driven
inputEngine.Mouse.MouseMove += OnMouseMove;
inputEngine.Controllers.TriggerPressed += OnTriggerPressed;
inputEngine.Keyboard.KeyDown += OnKeyDown;
// Update-driven
if (
    inputEngine.Keyboard.IsAnyKeyDown(Key.A, Key.Left)
    || inputEngine.Controllers.IsAnyCodeFlagged(0, (int) XboxButtonCode.Left, (int) XboxStickCode.LeftW)
) {
    character.X--;
}
if (inputEngine.Keyboard.AreAllKeysDown(Key.S, Key.C, Key.R, Key.T)) {
    UnlockEasterEgg();
}

...

do {
    inputEngine.UpdateControllers(); // Controller/DXInput events
    window.UpdateWindow(); // Keyboard/Mouse/Window events & Window properties
} while (true); // In reality you'll likely want to stop this when the last Window is closed
```
### Audio (NAudio)
```CSharp
ServiceLocator.ProvideService(typeof(AudioEngine)); // Also available: NullAudioEngine & LoggingAudioEngine
...
IAudioEngine audioEngine = ServiceLocator.GetService<IAudioEngine>();
IAudio audio = audioEngine.Add("example", AudioType.Music, AudioFormat.MP3, File.ReadAllBytes("music.mp3"));
audio.Play(true); // True = Repeat, False = Play only once

...

IAudio audio = audioEngine.Get("example");
audio.Volume = 0.15d;

...

Stream micStream = new MemoryStream();
audioEngine.StartRecording(ref micStream);
```
### Graphics (SFML)
```CSharp
Coming soon!
```
### Physics (Farseer)
```CSharp
Coming soon!
```
### Mod/Plugin Support
```CSharp
Coming soon!
```