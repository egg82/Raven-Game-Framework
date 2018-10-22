using System;

namespace Raven.Events {
    public class ExceptionEventArgs : EventArgs {
        // vars
        public static readonly new ExceptionEventArgs Empty = new ExceptionEventArgs(null);

        // constructor
        public ExceptionEventArgs(Exception ex) {
            Exception = ex;
        }

        // public
        public Exception Exception { get; private set; }

        // private

    }
}
