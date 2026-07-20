using UnityEngine;
using UnityEngine.UI;

namespace Backgammon.Game.UI {
    class Blocker : MonoBehaviour {
        static Canvas canvas;
        static Image image;
        void Awake() {
            canvas = GetComponent<Canvas>();
            image = GetComponent<Image>();
        }

        public static bool active {
            get => canvas.enabled;
            set => canvas.enabled = value;
        }
        public static Color color {
            get => image.color;
            set => image.color = value;
        }
    }
}