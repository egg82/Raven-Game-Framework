using log4net;
using Raven.Display;
using Raven.Input;
using Raven.Patterns;
using System.Collections.Immutable;
using System.Reflection;

namespace Test.Graphics {
    public class GraphicsState : State {
        //vars
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private Sprite ball = new Sprite();
        private IInputEngine inputEngine = ServiceLocator.GetService<IInputEngine>();

        //constructor
        public GraphicsState() : base() {
            
        }

        //public
        public override void Update(double deltaTime) {
            //log.Info("State update " + deltaTime);

            ball.Rotation += 0.05d;
            ball.X += 0.1d;

            //log.Info("Ball " + ball.X);

            ImmutableList<DisplayObject> objects = Window.QuadTree.Query(inputEngine.Mouse.Point.X, inputEngine.Mouse.Point.Y);
            foreach (DisplayObject obj in objects) {
                log.Info("Object under cursor " + obj);
            }

            base.Update(deltaTime);
        }

        //private
        protected override void Enter() {
            log.Info("State entered");

            ball.Width = 50.0d;
            ball.Height = 50.0d;

            Window.QuadTree.Add(ball);
            AddChild(ball);
        }
        protected override void Exit() {
            log.Info("State exited");
        }
    }
}
