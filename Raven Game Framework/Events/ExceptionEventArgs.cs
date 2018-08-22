using System;

namespace Raven.Events {
    public class ExceptionEventArgs : EventArgs {
        //vars
        public static readonly new ExceptionEventArgs Empty = new ExceptionEventArgs(null);

        private Exception ex = null;

        //constructor
        public ExceptionEventArgs(Exception ex) {
            this.ex = ex;
        }

        //public
        public Exception Exception {
            get {
                return ex;
            }
        }

        //private

    }
}
