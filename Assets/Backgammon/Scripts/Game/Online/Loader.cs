using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Backgammon.Game.Online {
    public class Loader : MonoBehaviour {
        [SerializeField] GameObject pause;
        void Awake() {
            Addressables.InstantiateAsync("prefabs/online/netmanager");
        }
    }
}