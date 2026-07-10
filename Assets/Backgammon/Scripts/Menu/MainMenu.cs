using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Backgammon.Menu {
    using Game;
    
    public class Main : MonoBehaviour {
        [SerializeField] Image blocker;

        IEnumerator Start() {
            var c = blocker.color;
            while (blocker.color.a > 0) {
                c.a -= Time.deltaTime * 4;
                blocker.color = c;
                yield return null;
            }
            c.a = 0;
            blocker.color = c;

            blocker.raycastTarget = false;
        }

        public void StartGame(int flag) => StartCoroutine(_StartGame(flag));
        IEnumerator _StartGame(int flag) {
            Bootstrap.flags[0] = (byte)flag;
            blocker.raycastTarget = true;

            var c = blocker.color;
            while (blocker.color.a < 1) {
                c.a += Time.deltaTime * 4;
                blocker.color = c;
                yield return null;
            }
            c.a = 1;
            blocker.color = c;

            SceneManager.LoadScene(1);
        }
        public void Exit() => StartCoroutine(_Exit());
        IEnumerator _Exit() {        
            blocker.raycastTarget = true;

            var c = blocker.color;
            while (blocker.color.a < 1) {
                c.a += Time.deltaTime * 4;
                blocker.color = c;
                yield return null;
            }
            c.a = 1;
            blocker.color = c;
        
            Application.Quit();
        }
    }
}