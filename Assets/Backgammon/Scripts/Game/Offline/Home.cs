using System.Linq;
using Extensions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Offline {
    public class Home : MoveTarget {
        [SerializeField] RectTransform rtransform;

        void Start() {
            _y = rtransform.rect.height/2;
            if (side == Cell.Side.Black) {
                _y *= -1;
                _y -= 40;
            } else _y += 40;
            _y = transform.position.y - _y;
        }

        protected override void SetTarget(bool ist) => _img.color = new Color(1, 1, 1, ist ? 0.02f : 0);

        public bool CanAdd(Cell c, int d) {
            if (c.side != side) return false;
            Vector2Int v = c.side == Cell.Side.White ? new(18, 23) : new(6, 11);
            if (!c.id.InRange(v.x, v.y)) return false;
            return c.id + d > v.y;
        }
        protected override bool OnCanAdd(Cell fc) {
            foreach (var d in Dice.dices.Where(d => d != 0))
                if (fc.id + d > (side == Cell.Side.White ? 23 : 11 )) return true;
            return false;
        }

        protected override void OnAdd(Transform checker) {
            checker.localPosition = new Vector3(0, (-rtransform.rect.center.y) + count * 10 - 218, 0);
        }

        public override void OnPointerEnter(PointerEventData _) {
            if ((Cell.Side)(GameManager.instance.iswturn ? 2 : 1) == side && _ist)
                targpos = new(transform.position.x, _y, 0);
        }

        protected override void OnTake() {}
        protected override void OnMove() {
            if (count == 15) GameManager.instance.End($"{(GameManager.instance.iswturn ? "White" : "Black")} wins!");
        }
    }
}