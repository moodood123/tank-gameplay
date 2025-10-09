using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject _playerPrefab;
    
    private static Dictionary<ulong, NetworkObject> _spawnedPlayers = new();

    private void Start()
    {
        Debug.Log("Load Complete");
        if (_spawnedPlayers.ContainsKey(OwnerClientId)) return;
        Debug.Log("Proceeding to spawn player");
        
        if (TryAutoSpawnPlayer(FindObjectsByType<Station>(FindObjectsSortMode.None).ToList(), out NetworkObject networkPlayer))
        {
            _spawnedPlayers[OwnerClientId] = networkPlayer;
        }
    }
    
    private bool TryAutoSpawnPlayer(List<Station> stations, out NetworkObject networkPlayer)
    {
        networkPlayer = null;
        GameObject go = Instantiate(_playerPrefab);

        if (!go.TryGetComponent(out PlayerController player) || !go.TryGetComponent(out NetworkObject no))
        {
            Debug.LogWarning("Invalid player prefab instantiated, cleaning up");
            Destroy(go);
            return false;
        }

        no.SpawnWithOwnership(OwnerClientId, true);
        
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
