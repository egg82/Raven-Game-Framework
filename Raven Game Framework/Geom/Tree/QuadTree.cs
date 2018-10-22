using JoshuaKearney.Collections;
using Raven.Geom.Tree.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Raven.Geom.Tree {
    public class QuadTree<T> : ICollection<T> where T : QuadNode {
        // vars
        private bool boundsChanged = false;
        private double width = 0.0d;
        private double height = 0.0d;
        private QuadLeaf<T> root = null;
        private readonly object rootLock = new object();

        private readonly ConcurrentSet<T> addedSet = new ConcurrentSet<T>();
        private readonly ConcurrentSet<T> removedSet = new ConcurrentSet<T>();
        private readonly ConcurrentSet<T> changedSet = new ConcurrentSet<T>();
        private readonly ConcurrentSet<T> objects = new ConcurrentSet<T>();

        // constructor
        public QuadTree(double width, double height) {
            if (double.IsNaN(width) || double.IsInfinity(width)) {
                throw new NotFiniteNumberException(width);
            }
            if (width <= 0.0d) {
                throw new ArgumentOutOfRangeException("width");
            }
            if (double.IsNaN(height) || double.IsInfinity(height)) {
                throw new NotFiniteNumberException(width);
            }
            if (height <= 0.0d) {
                throw new ArgumentOutOfRangeException("height");
            }

            root = new QuadLeaf<T>(0.0d, 0.0d, width, height);
            this.width = width;
            this.height = height;
        }

        // public
        public void Add(T item) {
            if (item == null) {
                throw new ArgumentNullException("item");
            }

            if (!objects.Add(item)) {
                return;
            }

            removedSet.Remove(item);
            addedSet.Add(item);
            changedSet.Add(item);
            item.BoundsChanged += OnBoundsChanged;
        }
        public bool Remove(T item) {
            if (item == null) {
                throw new ArgumentNullException("item");
            }

            if (!objects.Remove(item)) {
                return false;
            }

            item.BoundsChanged -= OnBoundsChanged;
            changedSet.Remove(item);
            addedSet.Remove(item);
            removedSet.Add(item);
            return true;
        }
        public void Clear() {
            foreach (T node in objects) {
                Remove(node);
            }
        }
        public bool Contains(T item) {
            return objects.Contains(item);
        }
        public int Count {
            get {
                return objects.Count;
            }
        }
        public bool IsReadOnly {
            get {
                return false;
            }
        }
        public void CopyTo(T[] array, int arrayIndex) {
            objects.CopyTo(array, arrayIndex);
        }
        public IEnumerator<T> GetEnumerator() {
            return objects.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator() {
            return objects.GetEnumerator();
        }

        public double Width {
            get {
                lock (rootLock) {
                    ReculculateNodeList();
                }
                return root.Width;
            }
            set {
                if (double.IsNaN(value) || double.IsInfinity(value)) {
                    throw new NotFiniteNumberException(value);
                }
                if (value <= 0.0d) {
                    throw new ArgumentOutOfRangeException("value");
                }

                lock (rootLock) {
                    if (value == root.Width) {
                        return;
                    }

                    width = value;
                    boundsChanged = true;
                }
            }
        }
        public double Height {
            get {
                lock (rootLock) {
                    ReculculateNodeList();
                }
                return root.Height;
            }
            set {
                if (double.IsNaN(value) || double.IsInfinity(value)) {
                    throw new NotFiniteNumberException(value);
                }
                if (value <= 0.0d) {
                    throw new ArgumentOutOfRangeException("value");
                }

                lock (rootLock) {
                    if (value == root.Height) {
                        return;
                    }

                    height = value;
                    boundsChanged = true;
                }
            }
        }

        public ImmutableList<T> Query(double x, double y, double width = 0.0d, double height = 0.0d) {
            if (double.IsNaN(x) || double.IsInfinity(x)) {
                throw new NotFiniteNumberException(x);
            }
            if (double.IsNaN(y) || double.IsInfinity(y)) {
                throw new NotFiniteNumberException(y);
            }
            if (double.IsNaN(width) || double.IsInfinity(width)) {
                throw new NotFiniteNumberException(width);
            }
            if (double.IsNaN(height) || double.IsInfinity(height)) {
                throw new NotFiniteNumberException(height);
            }

            ImmutableList<T> retVal = null;
            lock (rootLock) {
                ReculculateNodeList();
                retVal = ImmutableList.ToImmutableList(root.Query(x, y, (width < 0.0d) ? 0.0d : width, (height < 0.0d) ? 0.0d : height));
            }
            return retVal;
        }

        // private
        private void ReculculateNodeList() {
            if (boundsChanged) {
                root = new QuadLeaf<T>(0.0d, 0.0d, width, height);
                removedSet.Clear();

                foreach (T node in objects) {
                    changedSet.Remove(node);
                    addedSet.Remove(node);
                    root.Add(node);
                }
            } else {
                foreach (T node in addedSet) {
                    root.Add(node);
                    addedSet.Remove(node);
                }
                foreach (T node in changedSet) {
                    root.Move(node);
                    changedSet.Remove(node);
                }
                foreach (T node in removedSet) {
                    root.Remove(node);
                    removedSet.Remove(node);
                }
            }
        }
        
        private void OnBoundsChanged(object sender, BoundsEventArgs e) {
            changedSet.Add((T) sender);
        }
    }
}
