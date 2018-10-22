using Raven.Input.Enums;
using System;

namespace Raven.Input.Events {
    public class ButtonEventArgs : EventArgs {
        // vars
        public static readonly new ButtonEventArgs Empty = new ButtonEventArgs(-1, XboxButtonCode.None);

        // constructor
        public ButtonEventArgs(int controller, XboxButtonCode code) {
            Controller = controller;
            Code = code;
        }

        // public
        public int Controller { get; private set; }
        public XboxButtonCode Code { get; private set; }

        //private

    }
}
