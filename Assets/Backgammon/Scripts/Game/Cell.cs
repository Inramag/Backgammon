using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Backgammon.Game {
    public class Cell : MoveTarget  {
        public enum Side : byte { None, Black, White }

        int _mcount = 6;

        void Awake() {
            _y = 544 + 20 * (_id > 11 ? -1 : 1);
        }

        protected override void SetTarget(bool ist) => _img.gameObject.SetActive(ist);

        [SerializeField] int _id;
        public int id => _id;

        protected override bool OnCanAdd(Cell fc) {
            var c = 1;
            for (int i = _id; c < 6;) {
                i = i == 23 ? 0 : i + 1;

                var cell = GameManager.instance.cells[i];
                var b = cell.side == fc.side;
                
                if (!b || (cell == fc && fc.count == 0)) break;
                c++;
            }
            
            for (int i = _id; c < 6;) {
                i = i == 0 ? 23 : i - 1;

                var cell = GameManager.instance.cells[i];
                var b = cell.side == fc.side;
                
                if (!b || (cell == fc && fc.count == 0)) break;
                c++;
            }

            return c < 6;
        }
        protected override void OnAdd(Transform checker) {
            var ismax = count > _mcount;

            checker.localPosition = new(0, 192 - (ismax ? _mcount : count) * 64 + 32, 0);
            
            if (ismax) {
                checkers[^2].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
                checker.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = (count-_mcount+1).ToString();
            }
        }
        public Transform Take() {
            var checker = checkers[^1];
            checkers.RemoveAt(checkers.Count - 1);
            if (count > _mcount) {
                checkers[^1].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = (count-_mcount+1).ToString();
            }
            checker.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
            checker.transform.SetParent(transform.parent);
            return checker;
        }

        public override void OnPointerEnter(PointerEventData _) => targpos = new(transform.position.x, _y, 0);

        protected override void OnTake() {
            if (count != 0 && side == (Side)(GameManager.instance.iswturn ? 2 : 1) &&
                !(id == (side == Side.White ? 0 : 12) && GameManager.instance.isFromHead))
                StartCoroutine(GameManager.instance.StartMove(this));
        }
        protected override void OnMove() {}

        public static bool operator ==(Cell a, Cell b) => a.id == b.id;
        public static bool operator !=(Cell a, Cell b) => a.id != b.id;
        public override bool Equals(object obj) => obj is Cell c && c.id == id;
        public override int GetHashCode() => id.GetHashCode();
    }
}