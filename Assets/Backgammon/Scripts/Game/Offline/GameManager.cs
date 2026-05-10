using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Offline {
    public class GameManager : MonoBehaviour {
        public static GameManager instance;
        void Awake() => instance = this;

        [SerializeField] Image blocker;
        [SerializeField] GameObject endmenu;
        [SerializeField] TextMeshProUGUI endlbl;

        [SerializeField] GameObject b_checker, w_checker;

        public Home homeb, homew;
        [SerializeField] Transform p1, p2;

        public readonly Cell[] cells = new Cell[24];

        public bool isMoving;
        public Cell fromCell, toCell;
        public GameObject selectedChecker;

        bool iswfirst = true, isbfirst = true;
        bool isFromHead = false;

        public bool iswturn = true;
        bool isturncompl = false;
        bool isvictory = false;
        
        IEnumerator Start() {
            for (int i = 0; i < 24; i++) {
                cells[i] = transform.GetChild(i).GetComponent<Cell>();
            }

            var wcell = cells[0];
            var bcell = cells[12];
            for (int i = 0; i < 15; i++) {
                wcell.Add(Instantiate(w_checker));
                bcell.Add(Instantiate(b_checker));
            }

            wcell.side = 2;
            bcell.side = 1;

            cells[11].onClick = c => { if (c.side == 1) homeb.BearOff(c); };
            cells[23].onClick = c => { if (c.side == 2) homew.BearOff(c); };

            yield return StartCoroutine(Turn());
            iswfirst = false;
            yield return StartCoroutine(Turn());
            isbfirst = false;

            while (!isvictory) yield return StartCoroutine(Turn());

            yield return new WaitForSeconds(1f);
            endmenu.SetActive(true);
        }

        IEnumerator Turn() {
            yield return new WaitForSeconds(1f);

            yield return StartCoroutine(Dice.instance.Roll());

            var p = (iswturn ? p1 : p2).GetComponent<TextMeshProUGUI>();
            var ar = p.transform.GetChild(0).gameObject;

            p.color = new Color(1, 1, 1, 1);
            ar.SetActive(true);

            blocker.raycastTarget = false;
            while (!isturncompl) yield return null;
            blocker.raycastTarget = true;
            
            p.color = new Color(0.6f, 0.6f, 0.6f, 1);
            ar.SetActive(false);

            isturncompl = false;
            isFromHead = false;

            iswturn = !iswturn;
        }
        
        public IEnumerator StartMoveChecker(Cell cell) {
            if (cell.id == (iswturn ? 1 : 13) && isFromHead) yield break;

            isMoving = true;
            fromCell = cell;
            selectedChecker = cell.checkers[^1];
            cell.Remove();

            var dices = Dice.dices;

            if (Dice.isDouble) {
                int sum = 0;
                var d = dices[0];

                for (int i = 0; i < dices.Count; i++) {
                    sum += d;
                    var targ = cell.id + sum - 1;

                    if (iswturn ? (targ > 23) : (cell.id < 12 && targ > 11)) break;

                    var tcell = cells[targ % 24];

                    if (tcell.CanAdd(cell)) {
                        tcell.isTarget = true;
                        for (int j = 0; j <= i; j++)
                            tcell.usedDices.Add(d);
                    } else break;
                }
            } else {
                Cell tcell;
                int avail = 0;
                foreach (var d in dices) {
                    var targ = cell.id + d - 1;
                    if (iswturn ? (targ > 23) : (cell.id < 12 && targ > 11)) continue;

                    tcell = cells[targ % 24];
                    if (tcell.CanAdd(cell)) {
                        tcell.isTarget = true;
                        tcell.usedDices.Add(d);
                        avail++;
                    }
                }

                if (dices.Count == 2) {
                    var sum = cell.id - 1 + dices.Sum();
                    if (sum > 23) sum -= 24;

                    tcell = cells[sum];
                    if (avail != 0 && !(iswturn ? (sum > 23) : (cell.id < 12 && sum > 11))) {
                        if (tcell.CanAdd(cell)) {
                            tcell.isTarget = true;
                            tcell.usedDices.AddRange(dices);
                        }
                    }
                }
            }

            bool key = false;
            while (isMoving) {
                key = Keyboard.current.escapeKey.wasPressedThisFrame;
                if (key) {
                    isMoving = false;
                    cell.Add(selectedChecker);
                    break;
                }
                selectedChecker.transform.position = Cell.targpos;
                yield return null;
            }
            cell.side = cell.count == 0 ? (byte)0 : cell.side;
            selectedChecker = null;
            if (!key) {
                Dice.UseDices(toCell.usedDices);
                
                isFromHead = (cell.id == 1 && !iswfirst) || (cell.id == 13 && !isbfirst);
            }
            foreach (var c in cells) {
                c.isTarget = false;
                c.usedDices.Clear();
            }
            fromCell = null;
            toCell = null;
            if (dices.Count == 0) isturncompl = true;
        }

        public void End(string text) {
            isvictory = true;
            endlbl.text = text;
        }
    }
}