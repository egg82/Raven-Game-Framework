using Raven.Display.Core;
using Raven.Geom;
using Raven.Geom.Tree;
using SFML.Graphics;
using SFML.System;
using System;

namespace Raven.Display {
    public abstract class DisplayObject : QuadNode {
        //vars
        private volatile bool visible = true;
        private DisplayObjectContainer parent = null;
        
        private RenderStates renderState = RenderStates.Default;
        private VertexArray renderArray = new VertexArray(PrimitiveType.Quads, 4);

        private PointD local = new PointD();
        private PointD global = new PointD();
        private double rotation = 0.0d;

        //constructor
        public DisplayObject() : base() {
            TextureBounds = new RectD();
            Color = new Color(255, 255, 255, 255);
            Skew = new Skew();
            //Skew.Changed += OnSkewChanged;
            Scale = new PointD(1.0d, 1.0d);
            Scale.Changed += OnScaleChanged;
            TransformOffset = new PointD();
            TransformOffset.Changed += OnTransformChanged;
            Graphics = new Graphics();
            Graphics.Changed += OnGraphicsChanged;
        }

        //public
        public abstract void Update(double deltaTime);

        public bool Visible {
            get {
                return visible;
            }
            set {
                visible = value;
            }
        }
        
        public DisplayObjectContainer Parent {
            get {
                return parent;
            }
            internal set {
                parent = value;
                // TODO GlobalX & GlobalY should be re-checked when set is called
                // TODO Parent movement should be tracked to set new GlobalX & GlobalY
                ApplyBounds();
            }
        }
        public DisplayObject Root {
            get {
                DisplayObject root = Parent?.Root;
                return (root != null) ? root : this;
            }
        }
        
        public Graphics Graphics { get; }
        public RectD TextureBounds { get; }
        public Color Color { get; set; }
        public Skew Skew { get; }
        public PointD Scale { get; }
        public PointD TransformOffset { get; }
        public double Rotation {
            get {
                return rotation;
            }
            set {
                if (value == rotation) {
                    return;
                }
                if (double.IsNaN(value) || double.IsInfinity(value)) {
                    throw new NotFiniteNumberException(value);
                }

                double val = value;
                while (val < 0.0d) {
                    val += 360.0d;
                }
                while (val >= 360.0d) {
                    val -= 360.0d;
                }
                rotation = val;
            }
        }

        public Texture Texture {
            get {
                return renderState.Texture;
            }
            set {
                if (value == null) {
                    renderState.Texture = null;
                    TextureBounds.Width = 0.0d;
                    TextureBounds.Height = 0.0d;
                } else {
                    renderState.Texture = value;
                    TextureBounds.Width = value.Size.X;
                    TextureBounds.Height = value.Size.Y;
                }

                ApplyBounds();
            }
        }
        public BlendMode BlendMode {
            get {
                return renderState.BlendMode;
            }
            set {
                renderState.BlendMode = value;
                Graphics.BlendMode = value;
            }
        }
        public Shader Shader {
            get {
                return renderState.Shader;
            }
            set {
                renderState.Shader = value;
                Graphics.Shader = value;
            }
        }

        public double GlobalX {
            get {
                return global.X;
            }
            set {
                if (value == global.X) {
                    return;
                }
                if (double.IsNaN(value) || double.IsInfinity(value)) {
                    throw new NotFiniteNumberException(value);
                }
                
                double oldX = global.X;
                global.X = value;
                local.X += value - oldX;
                ApplyBounds();
            }
        }
        public override double X {
            get {
                return local.X;
            }
            set {
                if (value == local.X) {
                    return;
                }
                if (double.IsNaN(value) || double.IsInfinity(value)) {
                    throw new NotFiniteNumberException(value);
                }
                
                double oldX = local.X;
                local.X = value;

                double oldGlobalX = global.X;
                // TODO GlobalX should add parent's GlobalX when setting via local
                global.X += value - oldGlobalX;
                ApplyBounds();
            }
        }
        public override double Width {
            get {
                return base.Width;
            }
            set {
                if (value == base.Width) {
                    return;
                }
                if (double.IsNaN(value) || double.IsInfinity(value)) {
                    throw new NotFiniteNumberException(value);
                }

                double oldWidth = base.Width;
                Scale.X = value / oldWidth;
            }
        }
        public double GlobalY {
            get {
                return global.Y;
            }
            set {
                if (value == global.Y) {
                    return;
                }
                if (double.IsNaN(value) || double.IsInfinity(value)) {
                    throw new NotFiniteNumberException(value);
                }
                
                double oldY = global.Y;
                global.Y = value;
                local.Y += value - oldY;
                ApplyBounds();
            }
        }
        public override double Y {
            get {
                return local.Y;
            }
            set {
                if (value == local.Y) {
                    return;
                }
                if (double.IsNaN(value) || double.IsInfinity(value)) {
                    throw new NotFiniteNumberException(value);
                }
                
                double oldY = local.Y;
                local.Y = value;

                double oldGlobalY = global.Y;
                // TODO GlobalY should add parent's GlobalY when setting via local
                global.Y += value - oldGlobalY;
                ApplyBounds();
            }
        }
        public override double Height {
            get {
                return base.Height;
            }
            set {
                if (value == base.Height) {
                    return;
                }
                if (double.IsNaN(value) || double.IsInfinity(value)) {
                    throw new NotFiniteNumberException(value);
                }

                double oldHeight = base.Height;
                Scale.Y = value / oldHeight;
            }
        }

        //private
        internal virtual void Draw(RenderTarget target, Transform parentTransform, Color parentColor) {
            if (!visible) {
                return;
            }

            Transform globalTransform = parentTransform * GetTransform();
            Color globalColor = parentColor * Color;

            Graphics.Draw(target, parentTransform, globalColor, Skew, GlobalX, GlobalY, Rotation, TransformOffset);
            
            renderArray[0] = new Vertex(new Vector2f((float) Skew.TopLeft.X, (float) Skew.TopLeft.Y), globalColor, new Vector2f((float) TextureBounds.X, (float) TextureBounds.Y));
            renderArray[1] = new Vertex(new Vector2f((float) (TextureBounds.Width + Skew.TopRight.X), (float) Skew.TopRight.Y), globalColor, new Vector2f((float) (TextureBounds.X + TextureBounds.Width), (float) TextureBounds.Y));
            renderArray[2] = new Vertex(new Vector2f((float) (TextureBounds.Width + Skew.BottomRight.X), (float) (TextureBounds.Height + Skew.BottomRight.Y)), globalColor, new Vector2f((float) (TextureBounds.X + TextureBounds.Width), (float) (TextureBounds.Y + TextureBounds.Height)));
            renderArray[3] = new Vertex(new Vector2f((float) Skew.BottomLeft.X, (float) (TextureBounds.Height + Skew.BottomLeft.Y)), globalColor, new Vector2f((float) TextureBounds.X, (float) (TextureBounds.Y + TextureBounds.Height)));
            
            renderState.Transform = globalTransform;
            
            target.Draw(renderArray, renderState);
        }
        protected internal Transform GetTransform() {
            Transform retVal = Transform.Identity;

            retVal.Translate((float) (GlobalX + TransformOffset.X), (float) (GlobalY + TransformOffset.Y));
            retVal.Rotate((float) Rotation);
            // Always scale last, as it tends to screw with the other operations
            retVal.Scale((float) Scale.X, (float) Scale.Y, (float) -TransformOffset.X, (float) -TransformOffset.Y);

            return retVal;
        }
        private void ApplyBounds() {
            // TODO Think about possibly taking Skew into account. Lot of work and may not actually be worth it
            /*base.X = (global.X + Math.Min(Skew.TopLeft.X, Skew.BottomLeft.X)) * Scale.X;
            base.Y = (global.Y + Math.Min(Skew.TopLeft.Y, Skew.BottomLeft.Y)) * Scale.Y;
            base.Width = Math.Max((TextureBounds.Width + Math.Max(Skew.TopRight.X, Skew.BottomRight.X)) * Scale.X, Graphics.Width + Math.Max(Skew.TopRight.X, Skew.BottomRight.X));
            base.Height = Math.Max((TextureBounds.Height + Math.Max(Skew.BottomLeft.Y, Skew.BottomRight.Y)) * Scale.Y, Graphics.Height + Math.Max(Skew.BottomLeft.Y, Skew.BottomRight.Y));*/

            // TODO scale multiplier depends on (an inverse of?) TransformOffset and original Width/Height
            base.X = global.X * Scale.X;
            base.Y = global.Y * Scale.Y;
            base.Width = Math.Max(TextureBounds.Width * Scale.X, Graphics.Width);
            base.Height = Math.Max(TextureBounds.Height * Scale.Y, Graphics.Height);
        }

        /*private void OnSkewChanged(object sender, EventArgs e) {
            ApplyBounds();
        }*/
        private void OnScaleChanged(object sender, EventArgs e) {
            ApplyBounds();
        }
        private void OnTransformChanged(object sender, EventArgs e) {
            ApplyBounds();
        }
        private void OnGraphicsChanged(object sender, EventArgs e) {
            ApplyBounds();
        }
    }
}
