using JoshuaKearney.Collections;
using Raven.Core;
using SFML.Graphics;
using System;

namespace Raven.Display {
    public abstract class DisplayObjectContainer : DisplayObject {
        //vars
        private CopyOnWriteList<DisplayObject> painter = new CopyOnWriteList<DisplayObject>();
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
                child.Parent?.RemoveChild(child);
                child.Parent = this;
                painter.Add(child);
            }

            return true;
        }
        public bool AddChildAt(DisplayObject child, int index) {
            if (child == null) {
                throw new ArgumentNullException("child");
            }
            if (index < 0) {
                throw new ArgumentOutOfRangeException("index");
            }

            if (!children.Add(child)) {
                return false;
            }

            lock (childrenLock) {
                child.Parent?.RemoveChild(child);
                child.Parent = this;
                painter.Insert(index, child);
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
                painter.Remove(child);
                child.Parent = null;
            }

            return true;
        }
        public DisplayObject RemoveChildAt(int index) {
            if (index < 0) {
                throw new ArgumentOutOfRangeException("index");
            }

            DisplayObject child = painter[index];

            if (!children.Remove(child)) {
                return null;
            }

            lock (childrenLock) {
                painter.Remove(child);
                child.Parent = null;
            }

            return child;
        }
        public DisplayObject GetChild(int index) {
            if (index < 0 || index >= painter.Count) {
                return null;
            }

            return painter[index];
        }
        public T GetChild<T>(int index) where T : DisplayObject {
            return (T) GetChild(index);
        }
        public bool SetChildIndex(DisplayObject child, int newIndex) {
            if (child == null) {
                throw new ArgumentNullException("child");
            }
            if (newIndex < 0) {
                throw new ArgumentOutOfRangeException("newIndex");
            }

            if (!children.Contains(child)) {
                return false;
            }

            lock (childrenLock) {
                painter.Remove(child);
                painter.Insert(newIndex, child);
            }

            return true;
        }
        public int Children {
            get {
                return children.Count;
            }
        }

        public override void Update(double deltaTime) {
            foreach (DisplayObject obj in painter) {
                obj?.Update(deltaTime);
            }
        }

        //private
        internal override void Draw(RenderTarget target, Transform parentTransform, Color parentColor) {
            base.Draw(target, parentTransform, parentColor);

            Transform globalTransform = parentTransform * GetTransform();
            Color globalColor = parentColor * Color;
            
            if (painter.Count > 0) {
                for (int i = painter.Count - 1; i >= 0; i--) {
                    painter[i]?.Draw(target, globalTransform, globalColor);
                }
            }
        }
    }
}
