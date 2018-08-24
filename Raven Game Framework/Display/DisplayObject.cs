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
        
        private Color previousParentColor = Color.Transparent;
        private RenderStates renderState = RenderStates.Default;
        private VertexArray renderArray = new VertexArray(PrimitiveType.Quads, 4);

        private PointD local = new PointD();
        private double rotation = 0.0d;

        //constructor
        public DisplayObject() : base() {
            TextureBounds = new RectD();
            Color = new Color(255, 255, 255, 255);
            Skew = new Skew();
            Skew.Changed += OnSkewChanged;
            Scale = new PointD(1.0d, 1.0d);
            Scale.Changed += OnScaleChanged;
            TransformOffset = new PointD();
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

        public DisplayObjectContainer Parent { get; internal set; }
        public DisplayObject Root {
            get {
                DisplayObject root = Parent?.Root;
                return (root != null) ? root : this;
            }
        }
        
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
            }
        }
        public Shader Shader {
            get {
                return renderState.Shader;
            }
            set {
                renderState.Shader = value;
            }
        }

        public double GlobalX {
            get {
                return base.X;
            }
            set {
                double oldX = base.X;
                base.X = value;
                local.X += value - oldX;
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
                GlobalX += value - oldX;
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
                return base.Y;
            }
            set {
                double oldY = base.Y;
                base.Y = value;
                local.Y += value - oldY;
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
                GlobalX += value - oldY;
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
            
            Color globalColor = parentColor * Color;
            renderArray[0] = new Vertex(new Vector2f((float) Skew.TopLeft.X, (float) Skew.TopLeft.Y), globalColor, new Vector2f((float) TextureBounds.X, (float) TextureBounds.Y));
            renderArray[1] = new Vertex(new Vector2f((float) (TextureBounds.Width + Skew.TopRight.X), (float) Skew.TopRight.Y), globalColor, new Vector2f((float) (TextureBounds.X + TextureBounds.Width), (float) TextureBounds.Y));
            renderArray[2] = new Vertex(new Vector2f((float) (TextureBounds.Width + Skew.BottomRight.X), (float) (TextureBounds.Height + Skew.BottomRight.Y)), globalColor, new Vector2f((float) (TextureBounds.X + TextureBounds.Width), (float) (TextureBounds.Y + TextureBounds.Height)));
            renderArray[3] = new Vertex(new Vector2f((float) Skew.BottomLeft.X, (float) (TextureBounds.Height + Skew.BottomLeft.Y)), globalColor, new Vector2f((float) TextureBounds.X, (float) (TextureBounds.Y + TextureBounds.Height)));

            Transform globalTransform = parentTransform * GetTransform();
            renderState.Transform = globalTransform;
            
            target.Draw(renderArray, renderState);
        }
        protected internal Transform GetTransform() {
            Transform retVal = Transform.Identity;

            retVal.Translate((float) GlobalX, (float) GlobalY);
            retVal.Scale((float) Scale.X, (float) Scale.Y, (float) TransformOffset.X, (float) TransformOffset.Y);
            retVal.Rotate((float) Rotation, (float) TransformOffset.X, (float) TransformOffset.Y);

            return retVal;
        }
        private void ApplyBounds() {
            base.Width = (Math.Max(TextureBounds.Width, 1.0d) + Math.Max(Skew.TopRight.X, Skew.BottomRight.X)) * Scale.X;
            base.Height = (Math.Max(TextureBounds.Height, 1.0d) + Math.Max(Skew.BottomLeft.Y, Skew.BottomRight.Y)) * Scale.Y;
        }

        private void OnSkewChanged(object sender, EventArgs e) {
            ApplyBounds();
        }
        private void OnScaleChanged(object sender, EventArgs e) {
            ApplyBounds();
        }
    }
}
