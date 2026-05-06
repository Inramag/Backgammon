using System.Collections;
using System.Collections.Generic;
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

        [SerializeField] Dice diceSprites;
        [SerializeField] Image dice1, dice2;

        [SerializeField] GameObject b_checker, w_checker;

        public Home homeb, homew;

        readonly Cell[] cells = new Cell[24];
        bool isdouble = false;
        readonly List<int> dices = new();

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

            yield return StartCoroutine(Roll());

            blocker.raycastTarget = false;
            while (!isturncompl) yield return null;
            blocker.raycastTarget = true;

            isturncompl = false;
            iswturn = !iswturn;
        }

        

        public IEnumerator Roll() {
            dices.Clear();
            var dices1 = new List<int>();
            var dices2 = new List<int>();
            for (int i = 0; i < Random.Range(5, 15); i++) {
                dices1.Add(Random.Range(1, 7));
                dices2.Add(Random.Range(1, 7));
                Debug.Log($"Roll {i}: {dices1[i]} and {dices2[i]}");
            }
            for (int i = 0; i < dices1.Count; i++) {
                dice1.sprite = diceSprites[dices1[i]];
                dice2.sprite = diceSprites[dices2[i]];
                yield return new WaitForSeconds(i * 0.1f);
            }
            yield return new WaitForSeconds(0.6f);
            
            if (dices1[^1] == dices2[^1]) {
                dices.AddRange(new []{dices1[^1], dices1[^1], dices1[^1], dices1[^1]});
                isdouble = true;
            } else {
                dices.Add(dices1[^1]);
                dices.Add(dices2[^1]);
                isdouble = false;
            }
            Debug.Log($"Final dice values: {string.Join(", ", dices)}");
        }
        
        public IEnumerator StartMoveChecker(Cell cell) {
            if (cell.id == (iswturn ? 1 : 13) && isFromHead) yield break;

            isMoving = true;
            fromCell = cell;
            selectedChecker = cell.checkers[^1];
            cell.Remove();

            if (isdouble) {
                int sum = 0;
                var d = dices[0];

                for (int i = 0; i < dices.Count; i++) {
                    sum += d;
                    var targ = cell.id + sum - 1;

                    if (iswturn ? (targ > 23) : (cell.id < 12 && targ > 11)) break;

                    var tcell = cells[targ % 24];

                    if (tcell.CanAdd(cell.side)) {
                        tcell.isTarget = true;
                        for (int j = 0; j <= i; j++)
                            tcell.usedDices.Add(d);
                    }
                }
            } else {
                Cell tcell;
                int avail = 0;
                foreach (var d in dices) {
                    var targ = cell.id + d - 1;
                    if (iswturn ? (targ > 23) : (cell.id < 12 && targ > 11)) continue;

                    tcell = cells[targ % 24];
                    if (tcell.CanAdd(cell.side)) {
                        tcell.isTarget = true;
                        tcell.usedDices.Add(d);
                        avail++;
                    }
                }

                if (dices.Count == 2) {
                    var sum = cell.id + dices.Sum() - 1;
                    tcell = cells[sum % 24];
                    if (avail != 0 && !(iswturn ? (sum > 23) : (cell.id < 12 && sum > 11))) {
                        if (tcell.CanAdd(cell.side)) {
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
                foreach (var d in toCell.usedDices)
                    dices.Remove(d);
                
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