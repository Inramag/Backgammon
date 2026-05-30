using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Offline {
    public class Exit : MonoBehaviour {
        [SerializeField] Image blocker;

        void Update() {
            if (Keyboard.current.escapeKey.wasPressedThisFrame) No();
        }
        public void Yes() => StartCoroutine(_Yes());
        IEnumerator _Yes() {
            var c = blocker.color;
            while (blocker.color.a < 1) {
                c.a += Time.deltaTime * 4;
                blocker.color = c;
                yield return null;
            }
            c.a = 1;
            blocker.color = c;

            SceneManager.LoadScene(0);
            yield return null;
            StartCoroutine(MainMenu.instance.Back());
        }
        public void No() {
            GameManager.instance.enabled = true;
            gameObject.SetActive(false);
        }
    }
}