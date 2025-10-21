using Unity.Netcode;
using UnityEngine;

[DefaultExecutionOrder(-1000)]
public class TankSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject _tankPrefab;
    [SerializeField] private Transform _spawnPoint;
    
    private void Start()
    {
        if (IsServer) SpawnTank();
    }
    
    private void SpawnTank()
    {
        GameObject go = Instantiate(_tankPrefab, _spawnPoint.position, Quaternion.identity);

        if (go.TryGetComponent(out NetworkObject no) && go.TryGetComponent(out TankController tank))
        {
            no.Spawn();
        }
    }
}
