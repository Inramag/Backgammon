using TMPro;
using UnityEngine;

namespace Backgammon.Game.Offline {
    public class Player : MonoBehaviour {
        [SerializeField] TextMeshProUGUI _t;

        public bool active {
            set => _t.color = value ? new(1, 1, 1) : new(0.6f, 0.6f, 0.6f);
        }
    }
}