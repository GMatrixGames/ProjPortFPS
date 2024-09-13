using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    /// <summary>
    /// For some reason, Unity doesn't support using Dictionaries in the editor, nor serializing them into JSON.
    /// This implementation of a dictionary allows it to be used in editor, and (de-)serialized from/to JSON.
    /// </summary>
    /// <typeparam name="K">Key type</typeparam>
    /// <typeparam name="V">Value type</typeparam>
    [Serializable]
    public class SerializableDictionary<K, V> : Dictionary<K, V>, ISerializationCallbackReceiver
    {
        [SerializeField] private List<K> keys = new();
        [SerializeField] private List<V> vals = new();

        public void OnBeforeSerialize()
        {
            keys.Clear();
            vals.Clear();
            using var enumerator = GetEnumerator();
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                keys.Add(current.Key);
                vals.Add(current.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            Clear();
            for (var i = 0; i < keys.Count; i++)
            {
                Add(keys[i], vals[i]);
            }

            keys.Clear();
            vals.Clear();
        }
    }
}