using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;

namespace Backgammon.Game.Online {
    public class NetManager : MonoBehaviour {
        public static NetManager instance;
        static NetworkManager net;
        async void Awake() {
            instance = this;
            net = NetworkManager.Singleton;

            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        static async Task<string> Free() {
            string res = null;

            while (true) {
                var lobbies = await LobbyService.Instance.QueryLobbiesAsync(new QueryLobbiesOptions() {
                    Filters = new List<QueryFilter>() {
                        new(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.NE)
                    },
                    Count = 1
                });
                if (lobbies.Results.Count == 1) {
                    var lobby = lobbies.Results[0];
                    try { lobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id); }
                    catch (LobbyServiceException) { continue; }

                    res = lobby.Data["relay"].Value;
                }
                return res;
            }
        }

        static async Task<Allocation> Create() {
            var relay = await RelayService.Instance.CreateAllocationAsync(1);
            await LobbyService.Instance.CreateLobbyAsync("Inramag.Backgammon", 2, new CreateLobbyOptions() {
                Data = new Dictionary<string, DataObject>() {
                    {
                        "relay",
                        new(DataObject.VisibilityOptions.Member, await RelayService.Instance.GetJoinCodeAsync(relay.AllocationId))
                    }
                }
            });
            return relay;
        }
    }
}