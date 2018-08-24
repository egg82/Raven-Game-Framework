using System.Collections.Generic;

namespace Raven.Geom.Tree {
    class QuadLeaf<T> where T : QuadNode {
        //vars
        private QuadLeaf<T>[] leafs = new QuadLeaf<T>[4];
        private HashSet<T> objects = new HashSet<T>();

        private readonly QuadLeaf<T> parent = null;

        //constructor
        public QuadLeaf(double x, double y, double width, double height) : this(null, x, y, width, height) {
            
        }
        private QuadLeaf(QuadLeaf<T> parent, double x, double y, double width, double height) {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        //public
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Width { get; private set; }
        public double Height { get; private set; }
        
        public bool Add(T node) {
            if (parent == null) {
                return TryAdd(node);
            } else {
                if (!CompletelyContains(node)) {
                    return false;
                }
                return TryAdd(node);
            }
        }
        public bool Move(T node) {
            if (!Contains(node)) {
                // We did not have the node previously
                return false;
            }
            
            // We had the node previously
            if (parent != null && !CompletelyContains(node)) {
                // Node has moved out of our scope
                Remove(node);
                // Bubbles the "Add" function until it finds either the root or a leaf that completely contains the node
                return parent.BubbleAdd(node);
            }

            if (leafs[0] != null) {
                // Find the new leaf (if any) to put the node in
                foreach (QuadLeaf<T> leaf in leafs) {
                    if (leaf.Move(node)) {
                        return true;
                    }
                }
            }

            // The node didn't move past our own bounds, and we are the smallest leaf
            // OR the node now doesn't fit any leaf and we are the root
            return true;
        }
        public bool Remove(T node) {
            if (objects.Remove(node)) {
                return true;
            }

            bool retVal = false;

            if (leafs[0] != null) {
                bool leafsHaveObjects = false;
                foreach (QuadLeaf<T> leaf in leafs) {
                    if (leaf.Remove(node)) {
                        if (!leaf.Empty) {
                            //leafsHaveObjects = true; // Not needed
                            return true;
                        } else {
                            retVal = true;
                        }
                    } else {
                        if (!leaf.Empty) {
                            leafsHaveObjects = true;
                        }
                    }
                }
                if (!leafsHaveObjects) {
                    leafs = new QuadLeaf<T>[4];
                }
            }

            return retVal;
        }

        public HashSet<T> Query(double x, double y, double width, double height) {
            if (width == 0.0d || height == 0.0d) {
                if (ContainsPoint(X, Y, Width, Height, x, y)) {
                    // We contain the point
                    return GetIntersectingNodes(x, y, width, height);
                }
            }

            if (ContainsRect(x, y, width, height, X, Y, Width, Height)) {
                // We are contained in the search box
                return GetAllNodes();
            } else if (IntersectsRect(x, y, width, height, X, Y, Width, Height)) {
                // We intersect the search box
                return GetIntersectingNodes(x, y, width, height);
            } else {
                // We have nothing in this search box
                return new HashSet<T>();
            }
        }

        //private
        private bool Empty {
            get {
                return (objects.Count == 0 && leafs[0] == null) ? true : false;
            }
        }

        private HashSet<T> GetAllNodes() {
            HashSet<T> retVal = new HashSet<T>(objects);
            if (leafs[0] != null) {
                foreach (QuadLeaf<T> leaf in leafs) {
                    HashSet<T> items = leaf.GetAllNodes();
                    foreach (T node in items) {
                        retVal.Add(node);
                    }
                }
            }
            return retVal;
        }
        private HashSet<T> GetIntersectingNodes(double x, double y, double width, double height) {
            HashSet<T> retVal = new HashSet<T>();

            if (width == 0.0d || height == 0.0d) {
                foreach (T node in objects) {
                    if (ContainsPoint(node.X, node.Y, node.Width, node.Height, x, y)) {
                        retVal.Add(node);
                    }
                }
            } else {
                foreach (T node in objects) {
                    if (ContainsRect(x, y, width, height, node.X, node.Y, node.Width, node.Height) || IntersectsRect(x, y, width, height, node.X, node.Y, node.Width, node.Height)) {
                        retVal.Add(node);
                    }
                }
            }

            if (leafs[0] != null) {
                foreach (QuadLeaf<T> leaf in leafs) {
                    HashSet<T> items = leaf.GetIntersectingNodes(x, y, width, height);
                    foreach (T node in items) {
                        retVal.Add(node);
                    }
                }
            }

            return retVal;
        }

        private bool Contains(T node) {
            if (objects.Contains(node)) {
                return true;
            }
            if (leafs[0] == null) {
                return false;
            }

            foreach (QuadLeaf<T> leaf in leafs) {
                if (leaf.Contains(node)) {
                    return true;
                }
            }
            return false;
        }
        private bool CompletelyContains(T node) {
            if (node.X < X || node.Y < Y) {
                return false;
            }
            if (node.X + node.Width > X + Width || node.Y + node.Height > Y + Height) {
                return false;
            }
            return true;
        }

        private bool BubbleAdd(T node) {
            if (parent == null || CompletelyContains(node)) {
                return TryAdd(node);
            } else {
                return parent.BubbleAdd(node);
            }
        }
        private bool TryAdd(T node) {
            if (leafs[0] == null) {
                objects.Add(node);
                TrySubdivide();
            } else {
                foreach (QuadLeaf<T> leaf in leafs) {
                    if (leaf.Add(node)) {
                        return true;
                    }
                }
                objects.Add(node);
            }

            return true;
        }
        
        private void TrySubdivide() {
            if (objects.Count < 4) {
                return;
            }

            double newWidth = Width / 2.0d;
            double newHeight = Height / 2.0d;

            if (newWidth <= 1.0d || newHeight <= 1.0d) {
                return;
            }

            double midX = Width / 2.0d;
            double midY = Height / 2.0d;

            bool containsAny = false;
            foreach (T node in objects) {
                // Top left
                if (ContainsRect(X, Y, newWidth, newHeight, node.X, node.Y, node.Width, node.Height)) {
                    containsAny = true;
                    break;
                }
                // Top right
                if (ContainsRect(midX, Y, newWidth, newHeight, node.X, node.Y, node.Width, node.Height)) {
                    containsAny = true;
                    break;
                }
                // Bottom left
                if (ContainsRect(X, midY, newWidth, newHeight, node.X, node.Y, node.Width, node.Height)) {
                    containsAny = true;
                    break;
                }
                // Bottom right
                if (ContainsRect(midX, midY, newWidth, newHeight, node.X, node.Y, node.Width, node.Height)) {
                    containsAny = true;
                    break;
                }
            }

            if (containsAny) {
                leafs[0] = new QuadLeaf<T>(this, X, Y, newWidth, newHeight);
                leafs[1] = new QuadLeaf<T>(this, midX, Y, newWidth, newHeight);
                leafs[2] = new QuadLeaf<T>(this, X, midY, newWidth, newHeight);
                leafs[3] = new QuadLeaf<T>(this, midX, midY, newWidth, newHeight);
            }
        }

        private bool ContainsPoint(double x, double y, double width, double height, double px, double py) {
            return ((px >= x && px <= x + width) && (py >= y && py <= y + width)) ? true : false;
        }
        private bool ContainsRect(double x, double y, double width, double height, double rx, double ry, double rwidth, double rheight) {
            return (rx <= x && ry <= y && rx + rwidth <= x + width && ry + rheight <= y + height) ? true : false;
        }
        private bool IntersectsRect(double x, double y, double width, double height, double rx, double ry, double rwidth, double rheight) {
            return ((x + width >= rx && (y + height >= ry || ry + rheight > y)) || (y + height >= ry && (x + width >= rx || rx + rwidth > x))) ? true : false;
        }
    }
}
