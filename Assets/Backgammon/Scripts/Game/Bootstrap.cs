using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Backgammon.Game {
    public class Bootstrap : MonoBehaviour {
        [SerializeField] Transform canvas;
        public static byte[] flags = new byte[1];
        void Awake() {
            Addressables.InstantiateAsync($"prefabs/{(flags[0] == 0 ? "offline" : "online")}/board", canvas).WaitForCompletion();
        }
    }
}