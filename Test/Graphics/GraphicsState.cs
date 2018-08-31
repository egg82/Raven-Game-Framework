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
        private IInputEngine inputEngine = ServiceLocator.GetService<IInputEngine>();

        //constructor
        public GraphicsState() : base() {
            
        }

        //public
        public override void Update(double deltaTime) {
            //log.Info("State update " + deltaTime);

            // Multiply be deltaTime for constant-time calculation, divide by half because we actually double our update rate
            ball.Rotation += 1.0d * (deltaTime / 2.0d);
            ball.X += 0.2d * (deltaTime / 2.0d);

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
            ball.Color = new Color(255, 120, 120, 255);

            Window.QuadTree.Add(ball);
            AddChild(ball);
        }
        protected override void Exit() {
            log.Info("State exited");
        }
    }
}
