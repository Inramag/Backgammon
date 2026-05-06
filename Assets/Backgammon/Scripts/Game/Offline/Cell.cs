using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Offline {
    public class Cell : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler  {
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
        public List<int> usedDices = new(); 

        [SerializeField] int _id;
        public int id => _id;
        public byte side = 0; // 0 - empty, 1 - black, 2 - white
        public List<GameObject> checkers = new ();
        public byte count => (byte)checkers.Count;

        public Action<Cell> onClick;

        public bool CanAdd(byte s) => side == 0 || side == s;
        public void Add(GameObject checker) {
            checkers.Add(checker);
            checker.transform.SetParent(transform);
            var ismax = count > _mcount;
            checker.transform.localPosition = new Vector3(
                0, (isrev ? -1 : 1) * (192 - (ismax ? _mcount : count) * 64 + 32), 0
            );
            if (ismax) {
                checkers[^2].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
                checker.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ismax ? (count-_mcount+1).ToString() : "";
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
                if (GameManager.instance.fromCell == this) return;  

                var s = GameManager.instance.fromCell.side;          
                if (!isTarget) return;
                GameManager.instance.toCell = this;
                Add(GameManager.instance.selectedChecker);
                GameManager.instance.isMoving = false;
                side = s;
                onClick?.Invoke(this);
            } else if (count != 0 && side == (GameManager.instance.iswturn ? 2 : 1)) StartCoroutine(GameManager.instance.StartMoveChecker(this));
        }

        public static bool operator ==(Cell a, Cell b) => a.id == b.id;
        public static bool operator !=(Cell a, Cell b) => a.id != b.id;
        public override bool Equals(object obj) => obj is Cell c && c.id == id;
        public override int GetHashCode() => id.GetHashCode();
    }
}