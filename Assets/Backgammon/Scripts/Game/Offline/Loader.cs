using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Backgammon.Game.Offline {
    public class Loader : MonoBehaviour {
        [SerializeField] GameObject pause;
        void Awake() {
            Addressables.InstantiateAsync("prefabs/offline/board");
            pause.AddComponent<UI.Pause>();
        }
    }
}