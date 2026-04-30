using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Offline {
    public class GameManager : MonoBehaviour {
        public static GameManager instance;
        void Awake() => instance = this;

        public Image blocker;

        [SerializeField] Dice diceSprites;
        [SerializeField] Image dice1, dice2;

        public GameObject b_checker, w_checker;

        readonly Cell[] cells = new Cell[24];
        readonly List<int> dices = new();

        public bool isMoving;
        public Cell fromCell, toCell;
        public GameObject selectedChecker;

        public bool iswturn = true;
        bool isturncompl = false;
        
        IEnumerator Start() {
            for (int i = 0; i < 24; i++) {
                cells[i] = transform.GetChild(i).GetComponent<Cell>();
            }

            for (int i = 0; i < 15; i++) {
                cells[0].Add(Instantiate(w_checker));
                cells[12].Add(Instantiate(b_checker));
            }

            cells[0].side = 1;
            cells[12].side = 2;



            while (true) {
                yield return new WaitForSeconds(1f);

                yield return StartCoroutine(Roll());

                blocker.raycastTarget = false;
                while (!isturncompl) yield return null;
                blocker.raycastTarget = true;

                isturncompl = false;
                iswturn = !iswturn;
            }
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
                yield return new WaitForSeconds(-1 * (i - dices1.Count) * 0.1f);
            }
            yield return new WaitForSeconds(0.6f);
            
            if (dices1[^1] == dices2[^1]) {
                dices.AddRange(new []{dices1[^1], dices1[^1], dices1[^1], dices1[^1]});
            } else {
                dices.Add(dices1[^1]);
                dices.Add(dices2[^1]);
            }
            Debug.Log($"Final dice values: {string.Join(", ", dices)}");
        }
        
        public IEnumerator StartMoveChecker(Cell cell) {
            isMoving = true;
            fromCell = cell;
            selectedChecker = cell.checkers[^1];
            cell.Remove();

            foreach(var d in dices) {
                var targ = cell.id + d - 1;
                Debug.Log($"Checking move to cell {targ}");
                var tcell = cells[targ % 24];
                if (iswturn) {
                    if (targ > 23) continue;
                    if (tcell.CanAdd(cell.side)) tcell.isTarget = true;
                } else {
                    if (cell.id < 12 && targ > 11) continue;
                    if (tcell.CanAdd(cell.side)) tcell.isTarget = true;
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
            foreach (var c in cells) c.isTarget = false;
            if (!key) dices.Remove((toCell.id > fromCell.id) ? (toCell.id - fromCell.id) : (24 - fromCell.id + toCell.id));
            fromCell = null;
            toCell = null;
            if (dices.Count == 0) isturncompl = true;
        }
    }
}