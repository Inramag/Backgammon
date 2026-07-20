using Unity.Services.Relay;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Backgammon.Game.Online {
    public class Loader : MonoBehaviour {
        [SerializeField] GameObject pause;
        void Awake() {
            Addressables.InstantiateAsync("prefabs/online/netmanager").WaitForCompletion();
        }
        async void Start() {
            await NetManager.Initialize();

            var code = await LobbyManager.JoinFree();

            if (code == null) NetManager.StartHost(await LobbyManager.Create());
            else NetManager.StartClient(await RelayService.Instance.JoinAllocationAsync(code));
        }
    }
}