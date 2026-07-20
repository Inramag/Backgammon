using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace Backgammon.Game.Online {
    [RequireComponent(typeof(NetworkManager))]
    [RequireComponent(typeof(UnityTransport))]
    public class NetManager : MonoBehaviour {
        static NetworkManager net;
        static UnityTransport transport;

        void Awake() {
            net = GetComponent<NetworkManager>();
            transport = GetComponent<UnityTransport>();
        }

        public static async Task Initialize() {
            if (AuthenticationService.Instance.IsAuthorized) return;
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        public static void StartHost(Allocation alloc) {
            transport.SetRelayServerData(alloc.ToRelayServerData("dtls"));
            net.StartHost();
        }

        public static void StartClient(JoinAllocation alloc) {
            transport.SetRelayServerData(alloc.ToRelayServerData("dtls"));
            net.StartClient();
        }
    }
}