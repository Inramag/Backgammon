using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Backgammon.Game {
    public class Bootstrap : MonoBehaviour {
        [SerializeField] Transform canvas;
        void Awake() {
            Addressables.InstantiateAsync("prefabs/board", canvas).WaitForCompletion();
        }
    }
}