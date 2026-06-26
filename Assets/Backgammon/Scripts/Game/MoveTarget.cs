using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Backgammon.Game {
    public abstract class MoveTarget : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler {
        public Cell.Side side;
        
        public List<Transform> checkers = new ();
        public byte count => (byte)checkers.Count;

        public UsedDices usedDices = new();

        protected bool _ist;
        [SerializeField] protected Image _img;
        public bool istarget { get => _ist; set { _ist = value; SetTarget(value); } }
        protected abstract void SetTarget(bool ist);

        public void Add(GameObject checker) => Add(checker.transform);
        public void Add(Transform checker) {
            checkers.Add(checker);
            checker.SetParent(transform);
            OnAdd(checker);
        }
        protected abstract void OnAdd(Transform checker);

        public bool CanAdd(Cell fc) => (side == 0 || side == fc.side) && OnCanAdd(fc);
        protected abstract bool OnCanAdd(Cell _);

        public static Vector3 targpos;
        protected float _y;
        public abstract void OnPointerEnter(PointerEventData _);
        public void OnPointerClick(PointerEventData _) {
            if (GameManager.instance.isMoving) {
                if (!_ist) return;
                
                side = GameManager.instance.fcell.side;
                Add(GameManager.instance.selectedChecker);
                GameManager.instance.tcell = this;
                GameManager.instance.isMoving = false;

                OnMove();
            } else OnTake();
        }
        protected abstract void OnTake();
        protected abstract void OnMove();
    }
}