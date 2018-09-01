using log4net;
using Raven.Display;
using Raven.Input;
using Raven.Patterns;
using SFML.Graphics;
using System.Collections.Immutable;
using System.IO;
using System.Reflection;

namespace Test.Graphics {
    public class GraphicsState : State {
        //vars
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private Raven.Display.Sprite ball = new Raven.Display.Sprite();
        private Raven.Display.Sprite ball2 = new Raven.Display.Sprite();
        private IInputEngine inputEngine = ServiceLocator.GetService<IInputEngine>();

        private bool grow = false;
        private double value = 49.0d;

        //constructor
        public GraphicsState() : base() {
            
        }

        //public
        public override void Update(double deltaTime) {
            //log.Info("State update " + deltaTime);

            if (grow) {
                value++;
                if (value >= 49.0d) {
                    grow = false;
                }
            } else {
                value--;
                if (value <= 0.0d) {
                    grow = true;
                }
            }
            
            // Clear vs FillColor because then we'll have accurate Width, Height
            ball2.Graphics.Clear();
            ball2.Graphics.DrawEllipse(new System.Drawing.Pen(System.Drawing.Color.Red, 1.0f), (ball.Width - value - 1.0d) / 2.0d, (ball.Height - value - 1.0d) / 2.0d, value, value);
            // Multiply be deltaTime for constant-time calculation, divide by half because we actually double our update rate
            ball.Rotation += 1.0d * (deltaTime / 2.0d);
            ball.X += 0.2d * (deltaTime / 2.0d);
            ball.Y += 0.2d * (deltaTime / 2.0d);

            ImmutableList<DisplayObject> objects = Window.QuadTree.Query(inputEngine.Mouse.Point.X, inputEngine.Mouse.Point.Y);
            foreach (DisplayObject obj in objects) {
                log.Info("Object under cursor " + obj);
            }

            base.Update(deltaTime);
        }

        //private
        protected override void Enter() {
            log.Info("State entered");

            ball.Texture = new Texture(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar + "graphics" + Path.DirectorySeparatorChar + "ball.png");
            ball.Width = ball.Height = 50.0d;
            ball.TransformOffset.X = ball.Width / 2.0d;
            ball.TransformOffset.Y = ball.Height / 2.0d;
            ball.Color = new Color(120, 255, 120, 255);

            ball2.Graphics.DrawEllipse(new System.Drawing.Pen(System.Drawing.Color.Red, 1.0f), 0.0d, 0.0d, 49.0d, 49.0d);
            ball.TransformOffset.X = ball.Width / 2.0d;
            ball.TransformOffset.Y = ball.Height / 2.0d;
            ball2.Color = new Color(255, 255, 255, 120);

            /*ball2.Skew.TopLeft.X -= 20.0d;
            ball2.Skew.TopLeft.Y -= 20.0d;
            ball2.Skew.BottomRight.X += 20.0d;
            ball2.Skew.BottomRight.Y += 20.0d;*/

            Window.QuadTree.Add(ball);
            Window.QuadTree.Add(ball2);
            AddChild(ball);
            AddChild(ball2);
        }
        protected override void Exit() {
            log.Info("State exited");
        }
    }
}
