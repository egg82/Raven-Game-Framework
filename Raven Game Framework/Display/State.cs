namespace Raven.Display {
    public abstract class State : DisplayObjectContainer {
        //vars
        private volatile bool alive = false;
        private volatile bool active = true;

        //constructor
        public State() {

        }

        //public
        public Window Window { get; internal set; }
        internal bool Alive {
            get {
                return alive;
            }
        }
        public bool Active {
            get {
                return active;
            }
            set {
                active = value;
            }
        }

        //private
        internal void OnEnter() {
            Enter();
            alive = true;
        }
        internal void OnExit() {
            Exit();
            alive = false;
        }
        
        protected abstract void Enter();
        protected abstract void Exit();

        protected virtual void Resize(uint width, uint height) {

        }
    }
}
