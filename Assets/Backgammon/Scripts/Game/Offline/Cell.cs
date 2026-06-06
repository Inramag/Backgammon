using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Offline {
    public class Cell : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler  {
        [SerializeField] bool isrev;
        int _mcount = 6;

        [SerializeField] GameObject _targetImage;
        private bool _ist;
        public bool isTarget 
        {
            get => _ist;
            set {
                _ist = value;
                _targetImage.SetActive(value);
            }
        }
        public UsedDices usedDices = new();

        [SerializeField] int _id;
        public int id => _id;
        public byte side = 0;
        public List<GameObject> checkers = new ();
        public byte count => (byte)checkers.Count;

        public Action<Cell> onClick;

        public bool CanAdd(Cell fc) {
            var bs = side == 0 || side == fc.side;
            Debug.Log($"CanAdd ({fc.id} > {id}) > side {bs}");
            if (!bs) return false;

            var c = 1;
            for (int i = _id; c < 6;) {
                i = i == 23 ? 0 : i + 1;
                Debug.Log($"CanAdd > + id {i}");

                var cell = GameManager.instance.cells[i];
                var b = cell.side == fc.side;
                Debug.Log($"side {b}");
                
                if (!b || (cell == fc && fc.count == 0)) break;
                c++;
            }
            
            for (int i = _id; c < 6;) {
                i = i == 0 ? 23 : i - 1;
                Debug.Log($"CanAdd > - id {i}");

                var cell = GameManager.instance.cells[i];
                var b = cell.side == fc.side;
                Debug.Log($"side {b}");
                
                if (!b || (cell == fc && fc.count == 0)) break;
                c++;
            }

            return c < 6;
        }
        public void Add(GameObject checker) {
            checkers.Add(checker);

            var ismax = count > _mcount;

            var t = checker.transform;
            t.SetParent(transform);
            t.localPosition = new Vector3(
                0, (isrev ? -1 : 1) * (192 - (ismax ? _mcount : count) * 64 + 32), 0
            );
            
            if (ismax) {
                checkers[^2].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
                checker.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = (count-_mcount+1).ToString();
            }
        }
        public void Remove() {
            var checker = checkers[^1];
            checkers.RemoveAt(checkers.Count - 1);
            if (count > _mcount) {
                checkers[^1].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = (count-_mcount+1).ToString();
            }
            checker.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
            checker.transform.SetParent(transform.parent);
        }

        public static Vector3 targpos;
        public void OnPointerEnter(PointerEventData _) {
            targpos = new Vector3(
                transform.position.x,
                544 + 20 * (isrev ? -1 : 1),
                0
            );
        }
        public void OnPointerClick(PointerEventData _) {
            if (GameManager.instance.isMoving) {
                if (GameManager.instance.fcell == this) return;
                if (!isTarget) return;
                Add(GameManager.instance.selectedChecker);
                GameManager.instance.tcell = this;
                GameManager.instance.isMoving = false;
                side = GameManager.instance.fcell.side;
                onClick?.Invoke(this);
            } else if (
                count != 0 &&
                side == (GameManager.instance.iswturn ? 2 : 1) &&
                !(id == (side == 2 ? 0 : 12) && GameManager.instance.isFromHead))
                StartCoroutine(GameManager.instance.StartMove(this));
        }

        public static bool operator ==(Cell a, Cell b) => a.id == b.id;
        public static bool operator !=(Cell a, Cell b) => a.id != b.id;
        public override bool Equals(object obj) => obj is Cell c && c.id == id;
        public override int GetHashCode() => id.GetHashCode();
    }
}