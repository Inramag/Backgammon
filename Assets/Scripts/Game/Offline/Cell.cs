using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Offline {
    public class Cell : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler  {
        // отражение шашек
        [SerializeField] bool isrev;
        int _mcount = 6;

        [SerializeField] GameObject _targetImage; // Ссылка на стрелку в инспекторе
        public bool _isTarget;
        public bool isTarget 
        {
            get => _isTarget;
            set {
                _isTarget = value;
                _targetImage.SetActive(value);
            }
        }

        [SerializeField] int _id;
        public int id => _id;
        public byte side = 0; // 0 - empty, 1 - white, 2 - black
        public List<GameObject> checkers = new ();
        public byte count => (byte)checkers.Count;

        public bool CanAdd(byte s) {
            var b =  (side == 0 || side == s);
            return b;
        }
        public void Add(GameObject checker) {
            checkers.Add(checker);
            checker.transform.SetParent(transform);
            var ismax = count > _mcount;
            checker.transform.localPosition = new Vector3(
                0, (isrev ? -1 : 1) * (3 * 64 - (ismax ? _mcount : count) * 64 + 32), 0
            );
            if (ismax) {
                checkers[^2].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
                checker.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ismax ? (count - _mcount + 1).ToString() : "";
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
                544 + (isrev ? -10 : 10),
                0
            );
        }
        public void OnPointerClick(PointerEventData _) {
            if (GameManager.instance.isMoving) {
                if (GameManager.instance.fromCell == this) return;  

                var s = GameManager.instance.fromCell.side;          
                if (!isTarget) return;
                GameManager.instance.toCell = this;
                Add(GameManager.instance.selectedChecker);
                GameManager.instance.isMoving = false;
                side = s;
            } else if (count != 0) StartCoroutine(GameManager.instance.StartMoveChecker(this));
        }

        public static bool operator ==(Cell a, Cell b) => a.id == b.id;
        public static bool operator !=(Cell a, Cell b) => a.id != b.id;
        public override bool Equals(object obj) => obj is Cell c && c.id == id;
        public override int GetHashCode() => id.GetHashCode();
    }
}