using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Backgammon.Game {
    public class Bootstrap : MonoBehaviour {
        [SerializeField] GameObject pause;
        public static byte[] flags = new byte[1];

        void Awake() => Addressables.InstantiateAsync($"prefabs/o{(flags[0] == 0 ? "ff" : "n")}line/loader").WaitForCompletion();
    }
}