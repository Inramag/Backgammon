using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using Extensions;

namespace Backgammon.Game.Offline {
    public class Dice : MonoBehaviour {
        public static Dice instance { get; private set; }
        void Awake() => instance = this;

        [SerializeField] Transform _dices;
        [SerializeField] Image roller;

        public static bool isDouble { get; private set; }
        public static int[] dices = new int[4];

        public IEnumerator Roll() {
            for (int i = 0; i < 4; i++) dices[i] = 0;

            foreach (Transform d in _dices) d.gameObject.SetActive(false);

            roller.gameObject.SetActive(true);

            var ds1 = new List<byte>();
            var ds2 = new List<byte>();
            for (byte i = 1; i < UnityEngine.Random.Range(10, 20); i++) {
                while (true) {
                    var n = (byte)UnityEngine.Random.Range(1, 7);
                    if (ds1.Count == 0 || n != ds1[^1]) {
                        ds1.Add(n);
                        break;
                    }
                }
                while (true) {
                    var n = (byte)UnityEngine.Random.Range(1, 7);
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
                    yield return new WaitForSeconds(Math.Clamp(i, 5, 15) * 0.03f);
                    do1.sprite = this[ds1[i]];
                    do2.sprite = this[ds2[i]];
                }
                yield return new WaitForSeconds(0.8f);
            }

            isDouble = ds1.Last() == ds2.Last();
            if (isDouble) {
                for (int i = 0; i < 4; i++) dices[i] = ds1.Last();
            } else {
                dices[0] = ds1.Last();
                dices[1] = ds2.Last();
            }

            roller.gameObject.SetActive(false);
            FixDices();
        }

        public static void UseDices(UsedDices used) {
            if (dices.Eq(used.ToArray())) dices.Clear();
            else {
                foreach (var d in used) {
                    if (d == 0) break;
                    var i = dices.LastIndexOf(d);
                    dices[i] = 0;
                    if (!isDouble && i == 0) {
                        dices[0] = dices[1];
                        dices[1] = 0;
                    }
                }
            }

            used.clear();

            instance.FixDices();
        }

        public void FixDices() {
            var isempty = dices.All(d => d == 0);
            _dices.gameObject.SetActive(!isempty);

            if (!isempty) {
                if (isDouble) {
                    _dices.GetChild(0).gameObject.SetActive(false);

                    var dice = _dices.GetChild(1).GetComponent<RectTransform>();
                    dice.gameObject.SetActive(true);
                    dice.localPosition = new Vector3(-80, -60, 0);
                    dice.GetComponent<Image>().sprite = this[dices[0]];

                    var label = _dices.GetChild(2);
                    label.gameObject.SetActive(true);
                    label.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"{dices.Count(d => d != 0)}";
                } else {
                    _dices.GetChild(2).gameObject.SetActive(false);
                
                    var d1 = _dices.GetChild(1).GetComponent<RectTransform>();
                    d1.gameObject.SetActive(true);
                    d1.localPosition = new Vector3(0, -60, 0);
                    d1.GetComponent<Image>().sprite = this[dices[0]];

                    var d2 = _dices.GetChild(0).gameObject;
                    if (dices[1] == 0) {
                        d2.SetActive(false);
                    } else {
                        d2.SetActive(true);
                        d2.GetComponent<Image>().sprite = this[dices[1]];
                    }
                }
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)] static void Init() {
            static Sprite load(string n) => Addressables.LoadAssetAsync<Sprite>($"game[dice-{n}]").WaitForCompletion();
            for (int i = 0; i < 6;) _sprites[i] = load((++i).ToString());
        }
        static readonly Sprite[] _sprites = new Sprite[6];
        public Sprite this[int i] {
            get {
                if (i < 1 || i > 6) return null;
                return _sprites[i - 1];
            }
        }
    }
}