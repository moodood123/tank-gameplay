using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject _playerPrefab;

    private static Dictionary<ulong, NetworkObject> _spawnedPlayers = new Dictionary<ulong, NetworkObject>();

    private void Start()
    {
        if (IsServer) SpawnInitialPlayers();
    }

    private void SpawnInitialPlayers()
    {
        var clientIds = NetworkManager.Singleton.ConnectedClientsIds;

        List<Station> stations = FindObjectsByType<Station>(FindObjectsSortMode.None).ToList();

        foreach (ulong clientId in clientIds)
        {
            if (_spawnedPlayers.ContainsKey(clientId))
            {
                continue;
            }

            if (TryAutoSpawnPlayer(clientId, stations, out NetworkObject no))
            {
                _spawnedPlayers.Add(clientId, no);
            }
        }
    }
    
    private bool TryAutoSpawnPlayer(ulong ownerID, List<Station> stations, out NetworkObject networkPlayer)
    {
        networkPlayer = null;
        GameObject go = Instantiate(_playerPrefab);

        if (!go.TryGetComponent(out PlayerController player) || !go.TryGetComponent(out NetworkObject no))
        {
            Debug.LogWarning("Invalid player prefab instantiated, cleaning up");
            Destroy(go);
            return false;
        }

        no.SpawnWithOwnership(ownerID, true);
        
        foreach (Station station in stations)
        {
            if (player.Initialize(station.GetComponent<IPilotable>()))
            {
                networkPlayer = no;
                return true;
            }
        }

        return false;
    }
    
}
