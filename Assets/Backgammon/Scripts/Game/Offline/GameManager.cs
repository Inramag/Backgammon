using System.Collections;
using System.Linq;
using Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Offline {
    public class GameManager : MonoBehaviour {
        public static GameManager instance;

        [SerializeField] Image blocker;
        [SerializeField] GameObject exitmenu;
        [SerializeField] GameObject endmenu;
        [SerializeField] TextMeshProUGUI endlbl;

        [SerializeField] GameObject b_checker, w_checker;

        public Home homeb, homew;
        [SerializeField] TextMeshProUGUI p1, p2;

        public readonly Cell[] cells = new Cell[24];

        public bool isMoving;
        public Cell fcell;
        public MoveTarget tcell;
        public Transform selectedChecker;

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

            p1.color = inactpcolor;
            p2.color = inactpcolor;
        }

        void Update() {
            if (!endmenu.activeSelf && !isvictory && !isMoving && Keyboard.current.escapeKey.wasPressedThisFrame) {
                var isactive = !exitmenu.activeSelf;
                exitmenu.SetActive(isactive);
                blocker.raycastTarget = !isactive;
                Time.timeScale = isactive ? 0 : 1;
            }
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

            yield return wait(0.5f);

            var wcell = cells[23];
            var bcell = cells[11];
            wcell.side = Cell.Side.White;
            bcell.side = Cell.Side.Black;

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
            blocker.raycastTarget = false;
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
            isMoving = true;
            fcell = cell;
            selectedChecker = fcell.Take();

            var dices = Dice.dices.Where(d => d != 0).ToArray();

            var home = iswturn ? homew : homeb;
            {
                var hcells = new Vector2Int(iswturn ? 18 : 6, iswturn ? 23 : 11);
                var ishome = fcell.id.InRange(hcells.x, hcells.y);
                var allhome = false;
                if (ishome) {
                    allhome = true;
                    foreach(var c in cells) {
                        if (c.side != (Cell.Side)(iswturn ? 2 : 1)) continue;
                        if (!c.id.InRange(hcells.x, hcells.y)) {
                            allhome = false;
                            break;
                        }
                    }
                }
                if (allhome) {
                    if (Dice.isDouble) {
                        var sum = 0;
                        for (int i = 0; i < dices.Length; i++) {
                            sum += dices[0];
                            if (cell.id + sum > hcells.y) {
                                home.usedDices.Set(dices[0], i+1);
                                home.istarget = true;
                                break;
                            }
                        }
                    } else {
                        var dcs = new int[4];
                        foreach (var d in dices) {
                            if (cell.id + d > hcells.y) {
                                dcs[0] = dcs[0] == 0 ? d : (d < dcs[0] ? d : dcs[0]);
                            }
                        }
                        if (dcs[0] == 0 && dices.Length > 1) {
                            if (cell.id + dices.Sum() > hcells.y) {
                                dcs[0] = dices[0];
                                dcs[1] = dices[1];
                            }
                        }
                        if (dcs[0] != 0) {
                            home.usedDices.Set(dcs);
                            home.istarget = true;
                        }
                    }
                }
            }

            bool checktarg(Cell c, out Cell t, params int[] d) {
                if (CanMove(c, d.Sum(), out t) is var b && b) {
                    t.istarget = true;
                    t.usedDices.Set(d);
                }
                return b;
            }

            if (Dice.isDouble) {
                var sum = 0;
                var d = dices[0];

                for (int i = 1; i <= dices.Length; i++) {
                    sum += d;
                    if(!checktarg(fcell, out var tc, sum)) break;
                    tc.usedDices.Set(d, i);
                }
            } else {
                Cell tc;
                var av = 0;
                foreach(var d in dices) {
                    if (checktarg(fcell, out tc, d)) av++;
                }
                if (av > 0 && dices.Length > 1) {
                    checktarg(fcell, out tc, dices[0], dices[1]);
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

                selectedChecker.transform.position = MoveTarget.targpos;
                yield return null;
            }
            selectedChecker = null;

            if (fcell.count == 0) fcell.side = 0;
            if (!iskey) {
                tcell.usedDices.Use();
                
                isFromHead = (iswturn ? iswfirst : isbfirst) && (fcell.id == (iswturn ? 0 : 12)) ?
                    (Dice.isDouble ? fcell.count < 14 : true) : true;
            }

            foreach(var c in cells.Where(c => c.istarget)) {
                c.istarget = false;
                c.usedDices.Clear();
            }
            home.istarget = false;

            fcell = null;
            tcell = null;

            if (Dice.dices.All(d => d == 0)) isturncompl = true;
        }

        bool CanMove() {
            var home = iswturn ? homew : homeb;
            var allhome = true;
            {
                var hcells = new Vector2Int(iswturn ? 18 : 6, iswturn ? 23 : 11);
                foreach(var c in cells) {
                    if (c.side != (Cell.Side)(iswturn ? 2 : 1)) continue;
                    if (!c.id.InRange(hcells.x, hcells.y)) {
                        allhome = false;
                        break;
                    }
                }
            }

            foreach(var c in cells.Where(c => c.side == (Cell.Side)(iswturn ? 2 : 1))) {
                var b = CanMove(c);
                Debug.Log($"Can Move () > id {c.id}, {b}");
                if (b) return true;
            }

            if (allhome) {
                foreach(var c in cells.Where(c => c.side == (Cell.Side)(iswturn ? 2 : 1))) {
                    if (!Dice.isDouble) {
                        foreach(var d in Dice.dices) {
                            if (d == 0) break;
                            if (home.CanAdd(c, d)) return true;
                        }
                    }
                    var sum = 0;
                    foreach(var d in Dice.dices) {
                        if (d == 0) break;
                        sum += d;
                        if (home.CanAdd(c, sum)) return true;
                    }
                }
            }
            
            return false;
        }
        bool CanMove(Cell c) {
            foreach(var d in Dice.dices) {
                var b = CanMove(c, d);
                if (b) return true;
            }
            return false;
        }
        bool CanMove(Cell c, int d) {
            var t = c.id + d;
            var b1 = iswturn ? (t > 23) : (c.id < 12 && t > 11);
            t %= 24;
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
            isturncompl = true;
            isvictory = true;
            endlbl.text = text;
        }

        public void ExitYes() {
            StopAllCoroutines();
            Time.timeScale = 1;
            StartCoroutine(_ExitYes());
        }
        private IEnumerator _ExitYes() {
            var c = blocker.color;
            while (blocker.color.a < 1) {
                c.a += Time.deltaTime * 4;
                blocker.color = c;
                yield return null;
            }
            c.a = 1;
            blocker.color = c;

            SceneManager.LoadScene(0);
            yield return null;
        }
        public void ExitNo() {
            exitmenu.SetActive(false);
                Time.timeScale = 1;
        }
    }
}