using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;

namespace Backgammon.Game.Online {
    public static class LobbyManager {
        public static async Task<string> JoinFree() {
            while (true) {
                var lobbies = await LobbyService.Instance.QueryLobbiesAsync(new QueryLobbiesOptions() {
                    Filters = new List<QueryFilter>() {
                        new(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.NE)
                    },
                    Count = 1
                });

                if (lobbies.Results.Count == 0) return null;

                try {
                    return (await LobbyService.Instance.JoinLobbyByIdAsync(lobbies.Results[0].Id)).Data["relay"].Value;
                } catch (LobbyServiceException) {
                    continue;
                }
            }
        }

        public static async Task<Allocation> Create() {
            var alloc = await RelayService.Instance.CreateAllocationAsync(1);

            await LobbyService.Instance.CreateLobbyAsync(null, 2, new CreateLobbyOptions() {
                Data = new Dictionary<string, DataObject>() {
                    { "relay", new(DataObject.VisibilityOptions.Member, await RelayService.Instance.GetJoinCodeAsync(alloc.AllocationId)) }
                }
            });

            return alloc;
        }
    }
}