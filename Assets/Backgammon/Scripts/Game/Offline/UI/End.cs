using TMPro;
using UnityEngine;

namespace Backgammon.Game.Offline.UI {
    public class End : MonoBehaviour {
        static End instance;
        void Awake() => instance = this;
        
        [SerializeField] Canvas canvas;
        [SerializeField] TextMeshProUGUI tmpro;

        public static bool active => instance.canvas.enabled;

        public static void Finish(string winner) {
            instance.canvas.enabled = true;
            instance.tmpro.text = $"{winner} wins!";
        }
    }
}