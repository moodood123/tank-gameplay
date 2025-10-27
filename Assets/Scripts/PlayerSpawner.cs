using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(TankController))]
public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject _playerPrefab;

    private Dictionary<ulong, NetworkObject> _spawnedPlayers = new Dictionary<ulong, NetworkObject>();

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsServer) return;
        
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if (!IsServer) return;
        
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        // TODO: Add logic for spawning late joiners
        
        // SpawnPlayer(clientId);
    }
    
    public void SpawnInitialPlayers(List<Station> stations)
    {
        if (!IsServer) return;
        
        var clientIds = NetworkManager.Singleton.ConnectedClientsIds;

        foreach (ulong clientId in clientIds)
        {
            if (_spawnedPlayers.ContainsKey(clientId)) continue;

            SpawnPlayer(clientId, stations);
        }
    }

    public void SpawnPlayer(ulong clientId, List<Station> stations)
    {
        if (!IsServer) return;
        
        // Instantiate the player locally
        GameObject go = Instantiate(_playerPrefab);

        if (!go.TryGetComponent(out NetworkObject no) || !go.TryGetComponent(out PlayerController pc))
        {
            Debug.LogError("Attempted to instantiate invalid player prefab");
            Destroy(go);
            return;
        }

        // Spawn the player on the network
        no.SpawnWithOwnership(clientId, true);
        _spawnedPlayers[clientId] = no;
        
        // Assign the player an initial station
        foreach (Station station in stations)
        {
            if (!station.IsOccupied && station.TryEnter(pc))
            {
                pc.AssignStationClientRpc(station.NetworkObject);
                return;
            }
        }

        Debug.LogWarning("No available station found for the client");
    }

    public void DespawnPlayer(ulong clientId)
    {
        if(!IsServer) return;

        if (_spawnedPlayers.TryGetValue(clientId, out NetworkObject no))
        {
            no.Despawn();
            _spawnedPlayers.Remove(clientId);
        }
    }
}
