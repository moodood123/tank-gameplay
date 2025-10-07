using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerList : MonoBehaviour
{
    [SerializeField] private ScrollGroup _scrollGroup;
    [SerializeField] private GameObject _playerTilePrefab;

    private void Start()
    {
        // NetworkManager initializes unpredictably
        // Account for cases where the manager initializes later than OnEnable
        if (NetworkManager.Singleton.IsListening) Subscribe();
        else NetworkManager.Singleton.OnServerStarted += Subscribe;
    }

    private void OnDisable()
    {
        Unsubscribe();
    }
    
    private void Subscribe()
    {
        NetworkManager.Singleton.OnServerStarted -= Subscribe;
        
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    private void Unsubscribe()
    {
        if (!NetworkManager.Singleton) return;
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        ReassessPlayerList();
    }

    private void OnClientDisconnected(ulong clientId)
    {
        ReassessPlayerList();
    }

    private void ReassessPlayerList()
    {
        List<ulong> playerIds = new List<ulong>(NetworkManager.Singleton.ConnectedClientsIds);
        List<RectTransform> transforms = new List<RectTransform>();
        
        foreach (ulong playerId in playerIds)
        {
            GameObject go = Instantiate(_playerTilePrefab);

            if (!go.TryGetComponent(out RectTransform rectTransform) || !go.TryGetComponent(out PlayerTile tile))
            {
                Destroy(go);
                Debug.LogError("Invalid tile spawned");
                continue;
            }
            else
            {
                go.SetActive(true);
                tile.Setup(playerId);
                transforms.Add(rectTransform);
            }
        }

        _scrollGroup.SetupMenu(transforms);
    }

    private void OnDestroy()
    {
        if (!NetworkManager.Singleton) return;
        
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        
    }
}
