using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

using Backgammon.Game.UI;

namespace Backgammon.Game.Offline.UI {
    public class Pause : MonoBehaviour {
        Canvas canvas;
        void Awake() => canvas = GetComponent<Canvas>();
        
        void Update() {
            if (End.active) return;
            if (Manager.instance.isMoving) return;
            if (!Keyboard.current.escapeKey.wasPressedThisFrame) return;

            var curr = !canvas.enabled;
            canvas.enabled = curr;
            Blocker.active = !curr;
            Time.timeScale = curr ? 0 : 1;
        }

        public void ExitYes() {
            StopAllCoroutines();
            Time.timeScale = 1;
            StartCoroutine(_ExitYes());
        }
        private IEnumerator _ExitYes() {
            var c = Blocker.color;
            while (Blocker.color.a < 1) {
                c.a += Time.deltaTime * 4;
                Blocker.color = c;
                yield return null;
            }
            c.a = 1;
            Blocker.color = c;

            SceneManager.LoadScene(0);
            yield return null;
        }
        public void ExitNo() {
            canvas.enabled = false;
            Time.timeScale = 1;
        }
    }
}