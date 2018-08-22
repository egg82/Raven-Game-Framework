# Raven Game Framework
2D game framework written in C# that is designed to be easy to use, but still enforces good practices.
This is based off of the Starling/Sparrow projects. The idea is to make putting graphics on the screen extremely easy, along with handling input, audio, and physics.
Originally this project was called "Egg82LibEnhanced" but I scrapped both the project and the title as both were terrible.

# Usage
More examples available in the "Test" project
### Input (Keyboard, Mouse, DXInput)
```CSharp
ServiceLocator.ProvideService(typeof(InputEngine)); // Also available, NullInputEngine & LoggingInputEngine
...
IInputEngine inputEngine = ServiceLocator.GetService<IInputEngine>();
Window window = new Window("Input"); // Window is required for keyboard/mouse since that's what traps the events
inputEngine.AddWindow(window);

...

inputEngine.Mouse.MouseMove += OnMouseMove;
inputEngine.Controllers.TriggerPressed += OnTriggerPressed;
inputEngine.Keyboard.KeyDown += OnKeyDown;
if (inputEngine.Keyboard.IsAnyKeyDown(Key.A, Key.Left)) {
    character.X--;
}
if (inputEngine.Controllers.IsAnyCodeFlagged(0, (int) XboxButtonCode.Left, (int) XboxStickCode.LeftW)) {
    character.X--;
}
if (inputEngine.Keyboard.AreAllKeysDown(Key.S, Key.C, Key.R, Key.T)) {
    UnlockEasterEgg();
}

...

do {
    inputEngine.UpdateControllers(); // Controller/DXInput events
    window.Update(); // Keyboard/Mouse/Window events & Graphics Update() loop
} while (true); // In reality you'll likely want to stop this when the last Window is closed
```
### Audio (NAudio)
```CSharp
ServiceLocator.ProvideService(typeof(AudioEngine)); // Also available, NullAudioEngine & LoggingAudioEngine
...
IAudioEngine audioEngine = ServiceLocator.GetService<IAudioEngine>();
audio = audioEngine.Add("example", AudioType.Music, AudioFormat.MP3, File.ReadAllBytes("music.mp3"));
audio.Play(true); // True = Repeat, False = Play only once
```
### Graphics (SFML)
```CSharp
Coming soon!
```
### Physics (Farseer)
```CSharp
Coming soon!
```
### Mod Support
```CSharp
Coming soon!
```