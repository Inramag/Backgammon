using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Backgammon.Game {
    public class UsedDices : IEnumerable<int> {
        readonly int[] _dices = new int[4];
        
        public int this[int i] {
            get => _dices[i];
            set => _dices[i] = value;
        }

        public int[] get() => (int[])_dices.Clone();

        public void set(params int[] dices) {
            clear();
            for (int i = 0; i < Math.Min(_dices.Length, dices.Count()); i++)
                _dices[i] = dices[i];
        }
        public void set(int d, int c) {
            clear();
            for (int i = 0; i < c; i++)
                _dices[i] = d;
        }

        public void clear() => Array.Clear(_dices, 0, 4);

        public void use() {
            if (_dices[0] == 0) return;
            
            if (Bootstrap.flags[0] == 0) Offline.Dice.UseDices(this);
            else Online.Dice.UseDices(this);
        }

        public IEnumerator<int> GetEnumerator() => ((IEnumerable<int>)_dices).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}