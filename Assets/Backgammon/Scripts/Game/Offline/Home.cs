using UnityEngine;

namespace Offline {
    public class Home : MonoBehaviour {
        int count = 0;
        RectTransform rtransform;

        [SerializeField] string side;

        void Awake() => rtransform = GetComponent<RectTransform>();

        public void BearOff(Cell cell) {
            var c = cell.checkers[0].transform;
            cell.Remove();
            
            c.SetParent(transform);
            c.localPosition = new Vector3(0, (-rtransform.rect.center.y) + count++ * 10 + 32, 0);
            CheckVictory(count, side);
        }

        static void CheckVictory(int count, string side) {
            if (count == 15) GameManager.instance.End($"{side} виграли!");
        }
    }
}