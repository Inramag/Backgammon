using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Offline {
    public class GameManager : MonoBehaviour {
        public static GameManager instance;

        [SerializeField] Image blocker;
        [SerializeField] GameObject endmenu;
        [SerializeField] TextMeshProUGUI endlbl;

        [SerializeField] GameObject b_checker, w_checker;

        public Home homeb, homew;
        [SerializeField] TextMeshProUGUI p1, p2;

        public readonly Cell[] cells = new Cell[24];

        public bool isMoving;
        public Cell fcell, tcell;
        public GameObject selectedChecker;

        public bool iswfirst = true, isbfirst = true;
        public bool isFromHead = false;

        public bool iswturn = true;
        bool isturncompl = false;
        bool isvictory = false;

        // active player color / inactive player color
        readonly Color actpcolor = new(1, 1, 1), inactpcolor = new(0.6f, 0.6f, 0.6f);

        void Awake() {
            instance = this;

            for (int i = 0; i < 24; i++) {
                cells[i] = transform.GetChild(i).GetComponent<Cell>();
            }

            cells[11].onClick = c => { if (c.side == 1) homeb.BearOff(c); };
            cells[23].onClick = c => { if (c.side == 2) homew.BearOff(c); };

            cells[0].side = 2;
            cells[12].side = 1;

            p1.color = inactpcolor;
            p2.color = inactpcolor;
        }

        IEnumerator wait(float s) { yield return new WaitForSeconds(s); }

        IEnumerator Start() {
            var c = blocker.color;
            while (blocker.color.a > 0) {
                c.a -= Time.deltaTime * 2;
                blocker.color = c;
                yield return null;
            }
            c.a = 0;
            blocker.color = c;
            blocker.raycastTarget = true;

            yield return wait(0.5f);

            var wcell = cells[0];
            var bcell = cells[12];

            for (int i = 0; i < 15; i++) {
                yield return wait(0.1f);
                wcell.Add(Instantiate(w_checker));
                yield return wait(0.05f);
                bcell.Add(Instantiate(b_checker));
            }

            yield return wait(1f);

            p1.gameObject.SetActive(true);
            p2.gameObject.SetActive(true);

            yield return wait(1f);

            yield return StartCoroutine(Turn());
            iswfirst = false;
            yield return StartCoroutine(Turn());
            isbfirst = false;

            while (!isvictory) yield return StartCoroutine(Turn());

            yield return wait(1f);
            endmenu.SetActive(true);
        }

        IEnumerator Turn() {
            yield return wait(0.5f);
            yield return StartCoroutine(Dice.instance.Roll());
            if (!CanMove()) yield break;
            
            var p = iswturn ? p1 : p2;
            var ar = p.transform.GetChild(0).gameObject;

            p.color = actpcolor;
            ar.SetActive(true);
            blocker.raycastTarget = false;

            while(!isturncompl) yield return null;

            blocker.raycastTarget = true;
            p.color = inactpcolor;
            ar.SetActive(false);

            isturncompl = false;
            isFromHead = false;
            iswturn = !iswturn;
        }

        public IEnumerator StartMove(Cell cell) {
            if (cell.id == (iswturn ? 0 : 12) && isFromHead) yield break;

            isMoving = true;
            fcell = cell;
            selectedChecker = fcell.checkers[^1];
            fcell.Remove();

            var dices = Dice.dices;

            if (Dice.isDouble) {
                var sum = 0;
                var d = dices[0];

                for (int i = 0; i < dices.Count; i++) {
                    sum += d;
                    if(!CanMove(fcell, sum, out var tc)) break;

                    tc.isTarget = true;
                    for (int j = 0; j <= i; j++)
                        tc.usedDices[j] = d;
                }
            } else {
                Cell tc;
                var av = 0;
                foreach(var d in dices) {
                    if (CanMove(fcell, d, out tc)) {
                        av++;
                        tc.isTarget = true;
                        tc.usedDices[0] = d;
                    }
                }

                if (av > 0) {
                    if (CanMove(fcell, dices.Sum(), out tc)) {
                        tc.isTarget = true;
                        tc.usedDices = dices.ToArray();
                    }
                }
            }

            var iskey = false;
            while(isMoving) {
                iskey = Keyboard.current.escapeKey.wasPressedThisFrame;
                if (iskey) {
                    isMoving = false;
                    fcell.Add(selectedChecker);
                    break;
                }

                selectedChecker.transform.position = Cell.targpos;
                yield return null;
            }
            selectedChecker = null;

            if (fcell.count == 0) fcell.side = 0;
            if (!iskey) {
                Dice.UseDices(tcell.usedDices);
                
                if (!(iswturn ? iswfirst : isbfirst)) isFromHead = fcell.id == (iswturn ? 0 : 12);
            }

            foreach(var c in cells.Where(c => c.isTarget)) {
                c.isTarget = false;
                c.usedDices = new int[4];
            }

            fcell = null;
            tcell = null;

            if (Dice.dices.Count == 0) isturncompl = true;
        }

        bool CanMove() {
            foreach(var c in cells.Where(c => c.side == (iswturn ? 2 : 1))) {
                var b = CanMove(c);
                Debug.Log($"Can Move () > id {c.id}, {b}");
                if (b) return true;
            }
            return false;
        }
        bool CanMove(Cell c) {
            foreach(var d in Dice.dices) {
                var b = CanMove(c, d);
                Debug.Log($"Can Move (Cell) > dice {d}, {b}");
                if (b) return true;
            }
            return false;
        }
        bool CanMove(Cell c, int d) {
            var t = c.id + d;
            var b1 = iswturn ? (t > 23) : (c.id < 12 && t > 11);
            t %= 24;
            Debug.Log($"Can Move (Cell, dice) > {iswturn} ? ({t} > 23) : ({c.id} < 12 && {t} > 11)");
            Debug.Log($"Can Move (Cell, dice) > targ {t}, index test {b1}");
            if (b1) return false;
            return cells[t].CanAdd(c);
        }
        bool CanMove(Cell c, int d, out Cell tc) {
            tc = null;
            var t = c.id + d;
            if (iswturn ? (t > 23) : (c.id < 12 && t > 11)) return false;
            t %= 24;
            tc = cells[t];
            return tc.CanAdd(c);
        }

        public void End(string text) {
            isvictory = true;
            endlbl.text = text;
        }
        public IEnumerator Back() {
            var c = blocker.color;
            while (blocker.color.a < 1) {
                c.a += Time.deltaTime * 4;
                blocker.color = c;
                yield return null;
            }
            c.a = 1;
            blocker.color = c;

            SceneManager.LoadScene(0);
            StartCoroutine(MainMenu.instance.Back());
        }
    }
}