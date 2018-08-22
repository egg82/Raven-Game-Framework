using Raven.Input.Enums;
using System;

namespace Raven.Input.Events {
    public class ButtonEventArgs : EventArgs {
        //vars
        public static readonly new ButtonEventArgs Empty = new ButtonEventArgs(-1, XboxButtonCode.None);

        private readonly int controller = -1;
        private readonly XboxButtonCode code = XboxButtonCode.None;

        //constructor
        public ButtonEventArgs(int controller, XboxButtonCode code) {
            this.controller = controller;
            this.code = code;
        }

        //public
        public int Controller {
            get {
                return controller;
            }
        }
        public XboxButtonCode Code {
            get {
                return code;
            }
        }

        //private

    }
}
