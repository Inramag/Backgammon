using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    public static MainMenu instance;
    void Awake() => instance = this;
    
    [SerializeField] Camera _camera;

    [SerializeField] Image blocker;

    public void StartGame() => StartCoroutine(_StartGame());
    IEnumerator _StartGame() {
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
    public void Exit() => Application.Quit();
    public IEnumerator Back() {
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
}