using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;

public class Dice : MonoBehaviour {
    public static Dice instance { get; private set; }
    void Awake() => instance = this;

    [SerializeField] Transform _dices;
    [SerializeField] Image roller;

    public static bool isDouble { get; private set; }
    public static readonly List<int> dices = new();

    public IEnumerator Roll() {
        dices.Clear();

        foreach (Transform d in _dices) d.gameObject.SetActive(false);

        roller.gameObject.SetActive(true);

        var ds1 = new List<byte>();
        var ds2 = new List<byte>();
        for (byte i = 1; i < Random.Range(10, 20); i++) {
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

        {
            var do1 = roller.transform.GetChild(0).GetComponent<Image>();
            var do2 = roller.transform.GetChild(1).GetComponent<Image>();

            for (int i = 0; i < ds1.Count; i++) {
                yield return new WaitForSeconds(System.Math.Clamp(i, 5, 15) * 0.03f);
                do1.sprite = this[ds1[i]];
                do2.sprite = this[ds2[i]];
            }
            yield return new WaitForSeconds(0.8f);
        }

        isDouble = ds1.Last() == ds2.Last();
        if (isDouble) {
            for (int i = 0; i < 4; i++) dices.Add(ds1.Last());
        } else {
            dices.Add(ds1.Last());
            dices.Add(ds2.Last());
        }

        roller.gameObject.SetActive(false);
        FixDices();
    }

    public static void UseDices(List<int> n) {
        foreach (var dice in n) {
            dices.Remove(dice);
        }
        instance.FixDices();
    }

    void FixDices() {
        var isempty = dices.Count == 0;
        _dices.gameObject.SetActive(!isempty);

        if (!isempty) {
            if (isDouble) {
                _dices.GetChild(0).gameObject.SetActive(false);
                var d = _dices.GetChild(1).GetComponent<RectTransform>();
                d.gameObject.SetActive(true);
                var l = _dices.GetChild(2);
                l.gameObject.SetActive(true);
                l.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"{dices.Count}";
                d.localPosition = new Vector3(-80, -60, 0);
                d.GetComponent<Image>().sprite = this[dices[0]];
            } else {
                _dices.GetChild(2).gameObject.SetActive(false);
                var d1 = _dices.GetChild(1).GetComponent<RectTransform>();
                d1.gameObject.SetActive(true);
                d1.localPosition = new Vector3(0, -60, 0);
                    d1.GetComponent<Image>().sprite = this[dices[0]];
                if (dices.Count == 1) {
                    _dices.GetChild(0).gameObject.SetActive(false);
                } else {
                    var d2 = _dices.GetChild(0).GetComponent<RectTransform>();
                    d2.gameObject.SetActive(true);
                    d2.GetComponent<Image>().sprite = this[dices[1]];
                }
            }
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)] static void Init() {
        Sprite load(string n) => Addressables.LoadAssetAsync<Sprite>($"game[dice-{n}]").WaitForCompletion();
        for (int i = 0; i < 6;) _sprites[i] = load((++i).ToString());
    }
    [SerializeField] static Sprite[] _sprites = new Sprite[6];
    public Sprite this[int i] {
        get {
            if (i < 1 || i > 6) return null;
            return _sprites[i - 1];
        }
    }
}