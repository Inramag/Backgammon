using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dice : MonoBehaviour {
    public static Dice instance { get; private set; }
    void Awake() => instance = this;

    [SerializeField] Transform _dices;
    [SerializeField] Image roller;

    public readonly List<byte> dices = new();

    public IEnumerator Roll() {
        dices.Clear();
        byte d1, d2;

        foreach (GameObject d in _dices)
            d.SetActive(false);

        roller.gameObject.SetActive(true);

        var ds1 = new List<byte>();
        var ds2 = new List<byte>();
        for (byte i = 1; i < Random.Range(10, 30); i++) {
            while (true) {
                var n = (byte)Random.Range(1, 7);
                if (ds1.Count == 0 || n != ds1[^1]) {
                    ds1.Add(n);
                    break;
                }
            }
            while (true) {
                var n = (byte)Random.Range(1, 7);
                if (ds2.Count == 0 || n != ds2[^1]) {
                    ds2.Add(n);
                    break;
                }
            }
        }
        d1 = ds1[^1];
        d2 = ds2[^1];

        {
            var do1 = roller.transform.GetChild(0).GetComponent<Image>();
            var do2 = roller.transform.GetChild(1).GetComponent<Image>();

            for (int i = 0; i < ds1.Count; i++) {
                yield return new WaitForSeconds(i * 0.1f);
                do1.sprite = this[ds1[i]];
                do2.sprite = this[ds2[i]];
            }
        }

    }

    [RuntimeInitializeOnLoadMethod] static void Init() {
        _1 = Resources.Load<Sprite>("Dice/1");
        _2 = Resources.Load<Sprite>("Dice/2");
        _3 = Resources.Load<Sprite>("Dice/3");
        _4 = Resources.Load<Sprite>("Dice/4");
        _5 = Resources.Load<Sprite>("Dice/5");
        _6 = Resources.Load<Sprite>("Dice/6");
    }
    static Sprite _1, _2, _3, _4, _5, _6;
    public Sprite this[int i] {
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