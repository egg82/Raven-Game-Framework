using Raven.Geom;
using System;

namespace Raven.Display.Core {
    public class Skew {
        // events
        internal event EventHandler<EventArgs> Changed = null;

        // vars

        // constructor
        public Skew() {
            TopLeft = new PointD();
            TopRight = new PointD();
            BottomLeft = new PointD();
            BottomRight = new PointD();

            TopLeft.Changed += OnChanged;
            TopRight.Changed += OnChanged;
            BottomLeft.Changed += OnChanged;
            BottomRight.Changed += OnChanged;
        }

        // public
        public PointD TopLeft { get; private set; }
        public PointD TopRight { get; private set; }
        public PointD BottomLeft { get; private set; }
        public PointD BottomRight { get; private set; }

        // private
        private void OnChanged(object sender, EventArgs e) {
            Changed?.Invoke(this, e);
        }
    }
}
