using Raven.Utils;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Raven.Core {
    public class CopyOnWriteList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, IList, ICollection, IReadOnlyList<T>, IReadOnlyCollection<T> {
        //vars
        private volatile T[] backingArray = null;
        private volatile int index = 0;
        private readonly object backingLock = new object();

        //constructor
        public CopyOnWriteList() : this(0) {

        }
        public CopyOnWriteList(int initialCapacity) {
            if (initialCapacity < 0) {
                throw new ArgumentException("initialCapacity");
            }

            backingArray = new T[initialCapacity];
        }

        //public
        public virtual bool IsReadOnly {
            get {
                return false;
            }
        }
        public virtual bool IsFixedSize {
            get {
                return false;
            }
        }
        public object SyncRoot {
            get {
                return backingLock;
            }
        }
        public virtual bool IsSynchronized {
            get {
                return true;
            }
        }

        public int Add(object value) {
            if (value != null && !value.GetType().Equals(typeof(T)) && !value.GetType().IsAssignableFrom(typeof(T))) {
                throw new ArgumentException("value must be a valid type.");
            }

            int i = 0;
            lock (backingLock) {
                T[] temp = (index >= backingArray.Length) ? new T[MathUtil.UpperPowerOfTwo((uint) backingArray.Length + 1)] : new T[backingArray.Length];
                Array.Copy(backingArray, temp, backingArray.Length);
                backingArray = temp;
                backingArray[index++] = (T) value;
                i = index;
            }

            return index;
        }
        public void Add(T item) {
            lock (backingLock) {
                T[] temp = (index >= backingArray.Length) ? new T[MathUtil.UpperPowerOfTwo((uint) backingArray.Length + 1)] : new T[backingArray.Length];
                Array.Copy(backingArray, temp, backingArray.Length);
                backingArray = temp;
                backingArray[index++] = item;
            }
        }
        public void Insert(int index, object value) {
            if (value != null && !value.GetType().Equals(typeof(T)) && !value.GetType().IsAssignableFrom(typeof(T))) {
                throw new ArgumentException("value must be a valid type.");
            }
            Insert(index, (T) value);
        }
        public void Insert(int index, T item) {
            if (index < 0) {
                throw new ArgumentOutOfRangeException("index");
            }
            if (index >= this.index) {
                return;
            }

            lock (backingLock) {
                T[] temp = (index >= backingArray.Length) ? new T[MathUtil.UpperPowerOfTwo((uint) backingArray.Length + 1)] : new T[backingArray.Length];
                Array.Copy(backingArray, temp, backingArray.Length);

                int newIndex = index;
                for (int i = newIndex; i < temp.Length; i++) {
                    if (i - newIndex == 1) {
                        temp[i] = temp[newIndex];
                        newIndex = i;
                    }
                }
                temp[index] = item;

                backingArray = temp;
                this.index++;
            }
        }
        public void Remove(object value) {
            if (value != null && !value.GetType().Equals(typeof(T)) && !value.GetType().IsAssignableFrom(typeof(T))) {
                throw new ArgumentException("value must be a valid type.");
            }
            Remove((T) value);
        }
        public bool Remove(T item) {
            int reorderIndex = -1;

            lock (backingLock) {
                T[] temp = new T[backingArray.Length];
                Array.Copy(backingArray, temp, backingArray.Length);
                
                for (int i = 0; i < temp.Length; i++) {
                    if (reorderIndex == -1) {
                        bool? equals = temp[i]?.Equals(item);
                        if (equals.HasValue && equals.Value) {
                            temp[i] = default(T);
                            reorderIndex = i;
                        }
                    } else {
                        if (i - reorderIndex == 1) {
                            temp[reorderIndex] = temp[i];
                            temp[i] = default(T);
                            reorderIndex = i;
                        }
                    }
                }
                
                if (reorderIndex > -1) {
                    backingArray = temp;
                    index--;
                }
            }

            return (reorderIndex > -1) ? true : false;
        }
        public void RemoveAt(int index) {
            if (index < 0) {
                throw new ArgumentOutOfRangeException("index");
            }
            if (index >= this.index) {
                return;
            }

            lock (backingLock) {
                T[] temp = new T[backingArray.Length];
                Array.Copy(backingArray, temp, backingArray.Length);

                temp[index] = default(T);
                
                for (int i = index; i < temp.Length; i++) {
                    if (i - index == 1) {
                        temp[index] = temp[i];
                        temp[i] = default(T);
                        index = i;
                    }
                }
                
                backingArray = temp;
                this.index--;
            }
        }
        public void Clear() {
            lock (backingLock) {
                backingArray = new T[index = 0];
            }
        }
        public bool Contains(object value) {
            if (value != null && !value.GetType().Equals(typeof(T)) && !value.GetType().IsAssignableFrom(typeof(T))) {
                return false;
            }
            return Contains((T) value);
        }
        public bool Contains(T item) {
            T[] temp = backingArray;
            int tempIndex = index;

            for (int i = 0; i < tempIndex; i++) {
                bool? equals = temp[i]?.Equals(item);
                if (equals.HasValue && equals.Value) {
                    return true;
                }
            }

            return false;
        }
        public int IndexOf(object value) {
            if (value != null && !value.GetType().Equals(typeof(T)) && !value.GetType().IsAssignableFrom(typeof(T))) {
                return -1;
            }
            return IndexOf((T) value);
        }
        public int IndexOf(T item) {
            T[] temp = backingArray;
            int tempIndex = index;

            for (int i = 0; i < tempIndex; i++) {
                bool? equals = temp[i]?.Equals(item);
                if (equals.HasValue && equals.Value) {
                    return i;
                }
            }

            return -1;
        }
        public int Count {
            get {
                return index;
            }
        }

        object IList.this[int index] {
            get {
                if (index < 0) {
                    throw new ArgumentOutOfRangeException("arrayIndex");
                }

                T[] temp = backingArray;
                int tempIndex = this.index;

                return (index >= tempIndex) ? default(T) : temp[index];
            }
            set {
                if (value != null && !value.GetType().Equals(typeof(T)) && !value.GetType().IsAssignableFrom(typeof(T))) {
                    throw new ArgumentException("value must be a valid type.");
                }
                if (index < 0) {
                    throw new ArgumentOutOfRangeException("arrayIndex");
                }
                if (index >= this.index) {
                    return;
                }

                lock (backingLock) {
                    backingArray[index] = (T) value;
                }
            }
        }
        public T this[int index] {
            get {
                if (index < 0) {
                    throw new ArgumentOutOfRangeException("arrayIndex");
                }

                T[] temp = backingArray;
                int tempIndex = this.index;

                return (index >= tempIndex) ? default(T) : temp[index];
            }
        }
        T IList<T>.this[int index] {
            get {
                if (index < 0) {
                    throw new ArgumentOutOfRangeException("arrayIndex");
                }

                T[] temp = backingArray;
                int tempIndex = this.index;

                return (index >= tempIndex) ? default(T) : temp[index];
            }
            set {
                if (index < 0) {
                    throw new ArgumentOutOfRangeException("arrayIndex");
                }
                if (index >= this.index) {
                    return;
                }

                lock (backingLock) {
                    backingArray[index] = value;
                }
            }
        }

        public void CopyTo(Array array, int index) {
            if (array == null) {
                throw new ArgumentNullException("array");
            }
            if (!array.GetType().Equals(typeof(T[])) && !typeof(T[]).IsAssignableFrom(array.GetType())) {
                throw new ArgumentException("array must be a valid type.");
            }
            if (index < 0) {
                throw new ArgumentOutOfRangeException("index");
            }
            if (index >= array.Length) {
                throw new ArgumentException("index cannot be greater than or equal to array length.");
            }

            T[] temp = backingArray;
            int tempIndex = this.index;

            if (array.Length < index + tempIndex) {
                throw new ArgumentException("array cannot contain the elements in this list.");
            }

            Array.Copy(temp, 0, array, index, tempIndex);
        }
        public void CopyTo(T[] array, int arrayIndex) {
            if (array == null) {
                throw new ArgumentNullException("array");
            }
            if (arrayIndex < 0) {
                throw new ArgumentOutOfRangeException("arrayIndex");
            }
            if (arrayIndex >= array.Length) {
                throw new ArgumentException("arrayIndex cannot be greater than or equal to array length.");
            }

            T[] temp = backingArray;
            int tempIndex = index;

            if (array.Length < arrayIndex + tempIndex) {
                throw new ArgumentException("array cannot contain the elements in this list.");
            }

            Array.Copy(temp, 0, array, arrayIndex, tempIndex);
        }

        public IEnumerator<T> GetEnumerator() {
            T[] temp = backingArray;
            int tempIndex = index;

            for (int i = 0; i < tempIndex; i++) {
                yield return temp[i];
            }
        }
        IEnumerator IEnumerable.GetEnumerator() {
            T[] temp = backingArray;
            int tempIndex = index;

            for (int i = 0; i < tempIndex; i++) {
                yield return temp[i];
            }
        }

        //private

    }
}
