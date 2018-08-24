using JoshuaKearney.Collections;
using Raven.Geom.Tree.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Raven.Geom.Tree {
    public class QuadTree<T> : ICollection<T> where T : QuadNode {
        //vars
        private volatile Boolean boundsChanged = false;
        private QuadLeaf<T> root = null;
        private double width = 1.0d;
        private double height = 1.0d;
        private readonly object rootLock = new object();

        private ConcurrentSet<T> addedSet = new ConcurrentSet<T>();
        private ConcurrentSet<T> removedSet = new ConcurrentSet<T>();
        private ConcurrentSet<T> changedSet = new ConcurrentSet<T>();
        private ConcurrentSet<T> objects = new ConcurrentSet<T>();

        //constructor
        public QuadTree(double width, double height) {
            if (double.IsNaN(width) || double.IsInfinity(width)) {
                throw new NotFiniteNumberException(width);
            }
            if (width <= 0.0d) {
                throw new Exception("width must be positive and non-zero.");
            }
            if (double.IsNaN(height) || double.IsInfinity(height)) {
                throw new NotFiniteNumberException(width);
            }
            if (height <= 0.0d) {
                throw new Exception("height must be positive and non-zero.");
            }

            root = new QuadLeaf<T>(0.0d, 0.0d, width, height);
            this.width = width;
            this.height = height;
        }

        //public
        public void Add(T node) {
            if (node == null) {
                throw new ArgumentNullException("node");
            }

            if (!objects.Add(node)) {
                return;
            }

            removedSet.Remove(node);
            addedSet.Add(node);
            changedSet.Add(node);
            node.BoundsChanged += OnBoundsChanged;
        }
        public bool Remove(T node) {
            if (node == null) {
                throw new ArgumentNullException("node");
            }

            if (!objects.Remove(node)) {
                return false;
            }
            
            node.BoundsChanged -= OnBoundsChanged;
            changedSet.Remove(node);
            addedSet.Remove(node);
            removedSet.Add(node);
            return true;
        }
        public void Clear() {
            foreach (T node in objects) {
                Remove(node);
            }
        }
        public bool Contains(T node) {
            return objects.Contains(node);
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
                return root.Width;
            }
            set {
                if (double.IsNaN(value) || double.IsInfinity(value)) {
                    throw new NotFiniteNumberException(value);
                }
                if (value <= 0.0d) {
                    throw new Exception("value must be positive and non-zero.");
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
                return root.Height;
            }
            set {
                if (double.IsNaN(value) || double.IsInfinity(value)) {
                    throw new NotFiniteNumberException(value);
                }
                if (value <= 0.0d) {
                    throw new Exception("value must be positive and non-zero.");
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

            lock (rootLock) {
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

                return ImmutableList.ToImmutableList(root.Query(x, y, (width < 0.0d) ? 0.0d : width, (height < 0.0d) ? 0.0d : height));
            }
        }

        //private
        private void OnBoundsChanged(object sender, BoundsEventArgs e) {
            changedSet.Add((T) sender);
        }
    }
}
