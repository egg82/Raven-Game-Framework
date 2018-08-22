using System.Collections.Generic;

namespace Raven.Overrides {
    internal static class DictionaryOverride {
        //vars

        //public
        public static TValue AddIfAbsent<TKey, TValue>(this IDictionary<TKey, TValue> d, TKey key, TValue value) {
            TValue result = default(TValue);

            if (!d.TryGetValue(key, out result)) {
                d.Add(key, value);
                result = value;
            }

            return result;
        }

        //private

    }
}
