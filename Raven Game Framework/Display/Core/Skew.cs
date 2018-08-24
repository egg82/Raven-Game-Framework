using Raven.Geom;
using System;

namespace Raven.Display.Core {
    public class Skew {
        //vars
        internal event EventHandler<EventArgs> Changed = null;

        private PointD topLeft = new PointD();
        private PointD topRight = new PointD();
        private PointD bottomLeft = new PointD();
        private PointD bottomRight = new PointD();

        //constructor
        public Skew() {
            topLeft.Changed += OnChanged;
            topRight.Changed += OnChanged;
            bottomLeft.Changed += OnChanged;
            bottomRight.Changed += OnChanged;
        }

        //public
        public PointD TopLeft {
            get {
                return topLeft;
            }
        }
        public PointD TopRight {
            get {
                return topRight;
            }
        }
        public PointD BottomLeft {
            get {
                return bottomLeft;
            }
        }
        public PointD BottomRight {
            get {
                return bottomRight;
            }
        }

        //private
        private void OnChanged(object sender, EventArgs e) {
            Changed?.Invoke(this, e);
        }
    }
}
