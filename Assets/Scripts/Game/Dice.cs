using UnityEngine;

[System.Serializable] struct Dice {
    public Sprite _1, _2, _3, _4, _5, _6;
    
    public readonly Sprite this[int i] {
        get {
            return i switch {
                1 => _1,
                2 => _2,
                3 => _3,
                4 => _4,
                5 => _5,
                6 => _6,
                _ => null
            };
        }
    }
}