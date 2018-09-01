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
Window window = new Window(800, 600, "Graphics", SFML.Window.Styles.Default, false);
window.AddState(new GraphicsState()); // Anything that extends State

...

// In GraphicsState
private Raven.Display.Sprite ball = new Raven.Display.Sprite();

protected override void Enter() {
    // Optional image
    ball.Texture = new Texture(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar + "graphics" + Path.DirectorySeparatorChar + "ball.png");
    // Optional graphics, drawn directly onto the screen
    ball.Graphics.DrawEllipse(new System.Drawing.Pen(System.Drawing.Color.Red, 1.0f), 0.0d, 0.0d, ball.Width - 1.0d, ball.Height - 1.0d); // Taking into account the line width
    // Setting the TransformOffset so it rotates at its center
    ball.TransformOffset.X = ball.Width / 2.0d;
    ball.TransformOffset.Y = ball.Height / 2.0d;
}

public override void Update(double deltaTime) {
    ball.Rotation += 1.0d * deltaTime;
    ball.X += 0.2d * deltaTime;
    base.Update(deltaTime);
}
```
### Physics (Farseer)
```CSharp
Coming soon!
```
### Mod/Plugin Support
```CSharp
Coming soon!
```
### Musc Utilities/Tools
#### Update Loop
Automatically loops an arbitrary function in a new thread around a particular framerate. It constantly adjusts itself to better guarantee a stable and more accurate FPS.
```CSharp
UpdateDeltaLoop updateLoop = new UpdateDeltaLoop(delegate(double deltaTime) {
    ServiceLocator.GetService<IInputEngine>()?.UpdateControllers(); // Update controllers
    foreach (Window w in windows) {
        w.UpdateWindow(); // Update mouse/keyboard & fire events
        w.UpdateStates(deltaTime); // Update any states (and their child DisplayObjects) attached to the window
    }
}, 120.0d); // Update framerate
updateLoop.Start();
```
#### AffinityThread
A thread that has an affinity for a particular processor core. Use sparingly, as generally the OS knows better.
```CSharp
AffinityThread updateThread = new AffinityThread(threadStart);
updateThread.ProcessorCore = 1;
updateThread.Start();
```
#### QuadTree
QuadTrees come as part of the Window. Simply add any DisplayObject to it and you're off! It'll automatically re-calculate moved objects.
```CSharp
// In any State
Window.QuadTree.Add(ball);
...
// Don't actually need width/height if you just want to get objects under a point
ImmutableList<DisplayObject> objects = Window.QuadTree.Query(searchX, searchY, searchWidth, searchHeight);
foreach (DisplayObject obj in objects) {
    // Do something
}
```
#### Perlin/Simplex Noise
1D and 2D noise functions for everything from camera shakes to terrain generation and beyond!
```CSharp
INoise noise = new SimplexNoise(seed); // Or PerlinNoise
...
noise.Scale = intensity;
camera.Shake(noise.CalculateAll(x, width));
```