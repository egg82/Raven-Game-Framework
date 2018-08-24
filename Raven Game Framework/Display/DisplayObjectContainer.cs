using JoshuaKearney.Collections;
using Raven.Utils;
using SFML.Graphics;
using System;

namespace Raven.Display {
    public abstract class DisplayObjectContainer : DisplayObject {
        //vars
        private DisplayObject[] painter = new DisplayObject[0];
        private long index = 0;
        private ConcurrentSet<DisplayObject> children = new ConcurrentSet<DisplayObject>();
        private readonly object childrenLock = new object();

        //constructor
        public DisplayObjectContainer() : base() {

        }

        //public
        public bool AddChild(DisplayObject child) {
            if (child == null) {
                throw new ArgumentNullException("child");
            }

            if (!children.Add(child)) {
                return false;
            }

            lock (childrenLock) {
                // CoW
                DisplayObject[] temp = new DisplayObject[MathUtil.UpperPowerOfTwo((ulong) (painter.LongLength + 1L))];
                Array.Copy(painter, temp, painter.LongLength);
                temp[index] = child;
                index++;
                painter = temp;

                child.Parent?.RemoveChild(child);
                child.Parent = this;
            }

            return true;
        }
        public bool RemoveChild(DisplayObject child) {
            if (child == null) {
                throw new ArgumentNullException("child");
            }

            if (!children.Remove(child)) {
                return false;
            }

            lock (childrenLock) {
                // CoW
                DisplayObject[] temp = new DisplayObject[painter.LongLength];
                Array.Copy(painter, temp, painter.LongLength);

                long reorderIndex = -1L;
                for (long i = 0L; i < temp.LongLength; i++) {
                    if (temp[i] == child) {
                        temp[i] = null;
                        reorderIndex = i;
                    }
                    if (reorderIndex > -1L && i - reorderIndex == 1L) {
                        temp[reorderIndex] = temp[i];
                        temp[i] = null;
                        reorderIndex = i;
                    }
                }

                if (reorderIndex > -1L) {
                    index--;
                }
                painter = temp;

                child.Parent?.RemoveChild(child);
                child.Parent = this;
            }

            return true;
        }
        public DisplayObject GetChild(int index) {
            if (index < 0) {
                return null;
            }

            // CoW
            DisplayObject[] temp = painter;
            if (index >= temp.Length) {
                return null;
            }

            return temp[index];
        }
        public T GetChild<T>(int index) where T : DisplayObject {
            return (T) GetChild(index);
        }
        public int Children {
            get {
                return children.Count;
            }
        }

        public override void Update(double deltaTime) {
            // CoW
            DisplayObject[] temp = painter;
            foreach (DisplayObject obj in temp) {
                obj?.Update(deltaTime);
            }
        }

        //private
        internal override void Draw(RenderTarget target, Transform parentTransform, Color parentColor) {
            base.Draw(target, parentTransform, parentColor);

            Transform globalTransform = parentTransform * GetTransform();
            Color globalColor = parentColor * Color;

            // CoW
            DisplayObject[] temp = painter;
            long tempIndex = index;
            if (tempIndex > 0L) {
                for (long i = tempIndex - 1L; i >= 0L; i--) {
                    temp[i]?.Draw(target, globalTransform, globalColor);
                }
            }
        }
    }
}
