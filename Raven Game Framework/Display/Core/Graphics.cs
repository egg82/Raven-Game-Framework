using Raven.Geom;
using Raven.Utils;
using SFML.Graphics;
using SFML.System;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Raven.Display.Core {
    public class Graphics {
        // events
        internal event EventHandler<EventArgs> Changed = null;

        // vars
        private RenderStates renderState = RenderStates.Default;
        private readonly VertexArray renderArray = new VertexArray(PrimitiveType.Quads, 4);

        private Bitmap bitmap = new Bitmap(1, 1);
        private readonly object bitmapLock = new object();

        // constructor
        public Graphics() {
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap)) {
                g.Clear(System.Drawing.Color.Transparent);
            }
            renderState.Texture = TextureUtil.FromBitmap(bitmap);
        }

        // public
        public bool Antialiasing { get; set; }
        
        public void DrawArc(Pen pen, double x, double y, double width, double height, double startAngle, double sweepAngle) {
            if (pen == null) {
                throw new ArgumentNullException("pen");
            }

            lock (bitmapLock) {
                TryExpand(x, y, width, height, pen.Width);
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap)) {
                    g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
                    g.DrawArc(pen, (float) x, (float) y, (float) width, (float) height, (float) startAngle, (float) sweepAngle);
                }
                Texturize();
            }
        }

        public void DrawBezier(Pen pen, double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4) {
            if (pen == null) {
                throw new ArgumentNullException("pen");
            }

            lock (bitmapLock) {
                TryExpand(0.0d, 0.0d, Math.Max(Math.Max(Math.Max(x1, x2), x3), x4), Math.Max(Math.Max(Math.Max(y1, y2), y3), y4), pen.Width);
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap)) {
                    g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
                    g.DrawBezier(pen, (float) x1, (float) y1, (float) x2, (float) y2, (float) x3, (float) y3, (float) x4, (float) y4);
                }
                Texturize();
            }
        }
        public void DrawBeziers(Pen pen, PointD[] points) {
            if (pen == null) {
                throw new ArgumentNullException("pen");
            }
            if (points == null) {
                throw new ArgumentNullException("points");
            }

            lock (bitmapLock) {
                double maxX = 0.0d;
                double maxY = 0.0d;

                for (long i = 0L; i < points.LongLength; i++) {
                    if (points[i].X > maxX) {
                        maxX = points[i].X;
                    }
                    if (points[i].Y > maxY) {
                        maxY = points[i].Y;
                    }
                }

                TryExpand(0.0d, 0.0d, maxX, maxY, pen.Width);
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap)) {
                    g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
                    g.DrawBeziers(pen, ToPointFArray(points));
                }
                Texturize();
            }
        }

        public void DrawClosedCurve(Pen pen, PointD[] points, double tension, FillMode fillMode, bool fill = false) {
            if (pen == null) {
                throw new ArgumentNullException("pen");
            }
            if (points == null) {
                throw new ArgumentNullException("points");
            }

            lock (bitmapLock) {
                double maxX = 0.0d;
                double maxY = 0.0d;

                for (long i = 0L; i < points.LongLength; i++) {
                    if (points[i].X * tension > maxX) {
                        maxX = points[i].X * tension;
                    }
                    if (points[i].Y * tension > maxY) {
                        maxY = points[i].Y * tension;
                    }
                }

                TryExpand(0.0d, 0.0d, maxX, maxY, pen.Width);
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap)) {
                    g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
                    if (fill) {
                        g.FillClosedCurve(pen.Brush, ToPointFArray(points), fillMode, (float) tension);
                    } else {
                        g.DrawClosedCurve(pen, ToPointFArray(points), (float) tension, fillMode);
                    }
                }
                Texturize();
            }
        }
        public void DrawClosedCurve(Pen pen, PointD[] points, double offset, int numberOfSegments, double tension) {
            if (pen == null) {
                throw new ArgumentNullException("pen");
            }
            if (points == null) {
                throw new ArgumentNullException("points");
            }

            lock (bitmapLock) {
                double maxX = 0.0d;
                double maxY = 0.0d;

                for (long i = 0L; i < points.LongLength; i++) {
                    if ((points[i].X + offset) * tension > maxX) {
                        maxX = (points[i].X + offset) * tension;
                    }
                    if ((points[i].Y + offset) * tension > maxY) {
                        maxY = (points[i].Y + offset) * tension;
                    }
                }

                TryExpand(0.0d, 0.0d, maxX, maxY, pen.Width);
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap)) {
                    g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
                    g.DrawCurve(pen, ToPointFArray(points), (int) offset, numberOfSegments, (float) tension);
                }
                Texturize();
            }
        }

        public void DrawEllipse(Pen pen, double x, double y, double width, double height, bool fill = false) {
            if (pen == null) {
                throw new ArgumentNullException("pen");
            }

            lock (bitmapLock) {
                TryExpand(x, y, width, height, pen.Width);
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap)) {
                    g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
                    if (fill) {
                        g.FillEllipse(pen.Brush, (float) x, (float) y, (float) width, (float) height);
                    } else {
                        g.DrawEllipse(pen, (float) x, (float) y, (float) width, (float) height);
                    }
                }
                Texturize();
            }
        }

        public void DrawLine(Pen pen, double x1, double y1, double x2, double y2) {
            if (pen == null) {
                throw new ArgumentNullException("pen");
            }

            lock (bitmapLock) {
                TryExpand(0.0d, 0.0d, Math.Max(x1, x2), Math.Max(y1, y2), pen.Width);
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap)) {
                    g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
                    g.DrawLine(pen, (float) x1, (float) y1, (float) x2, (float) y2);
                }
                Texturize();
            }
        }
        public void DrawLines(Pen pen, PointD[] points) {
            if (pen == null) {
                throw new ArgumentNullException("pen");
            }
            if (points == null) {
                throw new ArgumentNullException("points");
            }

            lock (bitmapLock) {
                double maxX = 0.0d;
                double maxY = 0.0d;

                for (long i = 0L; i < points.LongLength; i++) {
                    if (points[i].X > maxX) {
                        maxX = points[i].X;
                    }
                    if (points[i].Y > maxY) {
                        maxY = points[i].Y;
                    }
                }

                TryExpand(0.0d, 0.0d, maxX, maxY, pen.Width);
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap)) {
                    g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
                    g.DrawLines(pen, ToPointFArray(points));
                }
                Texturize();
            }
        }

        public void DrawPie(Pen pen, double x, double y, double width, double height, double startAngle, double sweepAngle, bool fill = false) {
            if (pen == null) {
                throw new ArgumentNullException("pen");
            }

            lock (bitmapLock) {
                TryExpand(x, y, width, height, pen.Width);
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap)) {
                    g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
                    if (fill) {
                        g.FillPie(pen.Brush, (float) x, (float) y, (float) width, (float) height, (float) startAngle, (float) sweepAngle);
                    } else {
                        g.DrawPie(pen, (float) x, (float) y, (float) width, (float) height, (float) startAngle, (float) sweepAngle);
                    }
                }
                Texturize();
            }
        }

        public void DrawPolygon(Pen pen, PointD[] points, FillMode fillMode, bool fill = false) {
            if (pen == null) {
                throw new ArgumentNullException("pen");
            }
            if (points == null) {
                throw new ArgumentNullException("points");
            }

            lock (bitmapLock) {
                double maxX = 0.0d;
                double maxY = 0.0d;

                for (long i = 0L; i < points.LongLength; i++) {
                    if (points[i].X > maxX) {
                        maxX = points[i].X;
                    }
                    if (points[i].Y > maxY) {
                        maxY = points[i].Y;
                    }
                }

                TryExpand(0.0d, 0.0d, maxX, maxY, pen.Width);
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap)) {
                    g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
                    if (fill) {
                        g.FillPolygon(pen.Brush, ToPointFArray(points), fillMode);
                    } else {
                        g.DrawPolygon(pen, ToPointFArray(points));
                    }
                }
                Texturize();
            }
        }

        public void DrawRectangle(Pen pen, double x, double y, double width, double height, bool fill = false) {
            if (pen == null) {
                throw new ArgumentNullException("pen");
            }

            lock (bitmapLock) {
                TryExpand(x, y, width, height, pen.Width);
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap)) {
                    g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
                    if (fill) {
                        g.FillRectangle(pen.Brush, (float) x, (float) y, (float) width, (float) height);
                    } else {
                        g.DrawRectangle(pen, (float) x, (float) y, (float) width, (float) height);
                    }
                }
                Texturize();
            }
        }
        public void DrawRectangles(Pen pen, RectD[] rects, bool fill = false) {
            if (pen == null) {
                throw new ArgumentNullException("pen");
            }
            if (rects == null) {
                throw new ArgumentNullException("rects");
            }

            lock (bitmapLock) {
                double maxX = 0.0d;
                double maxY = 0.0d;

                for (long i = 0L; i < rects.LongLength; i++) {
                    if (rects[i].X + rects[i].Width > maxX) {
                        maxX = rects[i].X + rects[i].Width;
                    }
                    if (rects[i].Y + rects[i].Height > maxY) {
                        maxY = rects[i].Y + rects[i].Height;
                    }
                }

                TryExpand(0.0d, 0.0d, maxX, maxY, pen.Width);
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap)) {
                    g.SmoothingMode = (Antialiasing) ? SmoothingMode.AntiAlias : SmoothingMode.None;
                    if (fill) {
                        g.FillRectangles(pen.Brush, ToRectangleFArray(rects));
                    } else {
                        g.DrawRectangles(pen, ToRectangleFArray(rects));
                    }
                }
                Texturize();
            }
        }

        public void FillColor(System.Drawing.Color color) {
            lock (bitmapLock) {
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap)) {
                    g.Clear(color);
                }
                Texturize();
            }
        }
        public void Clear() {
            lock (bitmapLock) {
                if (bitmap.Width > 1 || bitmap.Height > 1) {
                    Bitmap oldBitmap = bitmap;
                    bitmap = new Bitmap(1, 1);
                    oldBitmap.Dispose();
                    Changed?.Invoke(this, EventArgs.Empty);
                }

                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap)) {
                    g.Clear(System.Drawing.Color.Transparent);
                }
                Texturize();
            }
        }

        // private
        internal void SetBlendMode(BlendMode blendMode) {
            renderState.BlendMode = blendMode;
        }
        internal void SetShader(Shader shader) {
            renderState.Shader = shader;
        }

        internal int Width {
            get {
                return bitmap.Width;
            }
        }
        internal int Height {
            get {
                return bitmap.Height;
            }
        }

        internal void Draw(RenderTarget target, Transform parentTransform, SFML.Graphics.Color globalColor, Skew skew, double globalX, double globalY, double rotation, PointD transformOffset) {
            renderArray[0] = new Vertex(new Vector2f((float) skew.TopLeft.X, (float) skew.TopLeft.Y), globalColor, new Vector2f(0.0f, 0.0f));
            renderArray[1] = new Vertex(new Vector2f((float) (renderState.Texture.Size.X + skew.TopRight.X), (float) skew.TopRight.Y), globalColor, new Vector2f((float) renderState.Texture.Size.X, 0.0f));
            renderArray[2] = new Vertex(new Vector2f((float) (renderState.Texture.Size.X + skew.BottomRight.X), (float) (renderState.Texture.Size.Y + skew.BottomRight.Y)), globalColor, new Vector2f((float) renderState.Texture.Size.X, (float) renderState.Texture.Size.Y));
            renderArray[3] = new Vertex(new Vector2f((float) skew.BottomLeft.X, (float) (renderState.Texture.Size.Y + skew.BottomLeft.Y)), globalColor, new Vector2f(0.0f, (float) renderState.Texture.Size.Y));

            Transform globalTransform = parentTransform * GetTransform(globalX, globalY, rotation, transformOffset);

            renderState.Transform = globalTransform;

            target.Draw(renderArray, renderState);
        }
        
        private void TryExpand(double x, double y, double width, double height, float penSize) {
            if (x + width + penSize > bitmap.Width || y + height + penSize > bitmap.Height) {
                Bitmap newBitmap = new Bitmap((int) Math.Max(Math.Ceiling(x + width + penSize), bitmap.Width), (int) Math.Max(Math.Ceiling(y + height + penSize), bitmap.Height));
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(newBitmap)) {
                    g.DrawImageUnscaled(bitmap, new Point(0, 0));
                }
                Bitmap oldBitmap = bitmap;
                bitmap = newBitmap;
                oldBitmap.Dispose();
                Changed?.Invoke(this, EventArgs.Empty);
            }
        }
        private void Texturize() {
            Texture oldTexture = renderState.Texture;
            renderState.Texture = TextureUtil.FromBitmap(bitmap);
            oldTexture.Dispose();
        }
        private PointF[] ToPointFArray(PointD[] points) {
            PointF[] retVal = new PointF[points.LongLength];
            for (long i = 0L; i < points.LongLength; i++) {
                retVal[i] = new PointF((float) points[i].X, (float) points[i].Y);
            }
            return retVal;
        }
        private RectangleF[] ToRectangleFArray(RectD[] rects) {
            RectangleF[] retVal = new RectangleF[rects.LongLength];
            for (long i = 0L; i < rects.LongLength; i++) {
                retVal[i] = new RectangleF((float) rects[i].X, (float) rects[i].Y, (float) rects[i].Width, (float) rects[i].Height);
            }
            return retVal;
        }

        private Transform GetTransform(double globalX, double globalY, double rotation, PointD transformOffset) {
            Transform retVal = Transform.Identity;

            retVal.Translate((float) globalX, (float) globalY);
            retVal.Rotate((float) rotation, (float) transformOffset.X, (float) transformOffset.Y);

            return retVal;
        }
    }
}
