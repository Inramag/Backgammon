using System;
using System.Collections.Generic;
using Unity.Netcode;

namespace Backgammon.Game.Online.Net {
    public class NetArray<T> {
        NetworkVariable<T>[] _data;
        public int length => _data.Length;

        public NetArray(int length) {
            _data = new NetworkVariable<T>[length];
            for (int i = 0; i < length; i++)
                _data[i] = new NetworkVariable<T>();
        }

        public T this[int i] {
            get => _data[i].Value;
            set => _data[i].Value = value;
        }

        public int Count(Func<T, bool> func) {
            int i = 0;
            foreach(var variable in _data)
                if (func(variable.Value)) i++;
            return i;
        }
        public bool All(Func<T, bool> func) {
            foreach(var variable in _data)
                if (!func(variable.Value)) return false;
            return true;
        }

        public void Clear(T value = default!) {
            foreach(var el in _data)
                el.Value = value;
        }

        public int LastIndexOf(T _el) {
            for (int i = length-1; i >= 0; i--)
                if (EqualityComparer<T>.Default.Equals(_data[i].Value, _el))
                    return i;
            return -1;
        }

        public static bool operator ==(NetArray<T> a, T[] b) {
            if (a.length != b.Length) return false;
            for (int i = 0; i < a.length; i++)
                if (!EqualityComparer<T>.Default.Equals(a[i], b[i])) return false;
            return true;
        }
        public static bool operator !=(NetArray<T> a, T[] b) => !(a == b);
    }
}