using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ExtractionZone : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _extractionDelay;
    
    private Dictionary<TankController, Coroutine> _tankExtractionTimers = new Dictionary<TankController, Coroutine>();
    
    public delegate void OnTankExtractionComplete(TankController tankController);
    public event OnTankExtractionComplete onTankExtractionComplete;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out TankController tank))
        {
            if (_tankExtractionTimers.TryGetValue(tank, out var existingCoroutine))
            {
                StopCoroutine(existingCoroutine);
            }

            Coroutine newCoroutine = StartCoroutine(ExtractionTimer(tank));
            _tankExtractionTimers[tank] = newCoroutine;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out TankController tank))
        {
            if (_tankExtractionTimers.ContainsKey(tank))
            {
                StopCoroutine(_tankExtractionTimers[tank]);
                _tankExtractionTimers.Remove(tank);
            }
        }
    }

    private void OnTankExtracted(TankController tank)
    {
        Debug.Log("Extracted tank: " + tank.transform.name);
        onTankExtractionComplete?.Invoke(tank);
        _tankExtractionTimers?.Remove(tank);
    }

    private IEnumerator ExtractionTimer(TankController tank)
    {
        yield return new WaitForSeconds(_extractionDelay);
        OnTankExtracted(tank);
    }
}
