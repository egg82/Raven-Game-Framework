using System.Threading;

namespace Raven.Display {
    public abstract class State : DisplayObjectContainer {
        // vars
        private int alive = 0;
        private int active = 1;

        // constructor
        protected State() : base() {

        }

        // public
        public Window Window { get; internal set; }
        internal bool Alive {
            get {
                return alive != 0;
            }
        }
        public bool Active {
            get {
                return active != 0;
            }
            set {
                Interlocked.Exchange(ref active, value ? 1 : 0);
            }
        }

        // private
        internal void OnEnter() {
            Enter();
            Interlocked.Exchange(ref alive, 1);
        }
        internal void OnExit() {
            Exit();
            Interlocked.Exchange(ref alive, 0);
        }
        
        protected abstract void Enter();
        protected abstract void Exit();

        protected virtual void Resize(uint width, uint height) {

        }
    }
}
