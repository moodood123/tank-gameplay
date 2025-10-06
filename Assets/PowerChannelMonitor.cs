using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerChannelMonitor : MonoBehaviour
{
    [SerializeField] private PowerRelay _relay;
    [SerializeField] private float _updateDelay = 1f;
    [SerializeField] private List<Pairing> _pairings = new List<Pairing>();

    private void Start()
    {
        StartCoroutine(UpdateLoop());
    }
    
    private IEnumerator UpdateLoop()
    {
        while (true)
        {
            yield return null;
            Debug.Log("Engine: " + _relay.PowerRatios[PowerChannel.Engine]);
            Debug.Log("Lighting: " + _relay.PowerRatios[PowerChannel.Lighting]);
            Debug.Log("Turret: " + _relay.PowerRatios[PowerChannel.Turret]);
            foreach (var pairing in _pairings)
            {
                pairing.FillImage.fillAmount = _relay.PowerRatios[pairing.Channel];
            }
            yield return new WaitForSeconds(_updateDelay);
        }
    }

    [Serializable]
    private class Pairing
    {
        [field: SerializeField] public PowerChannel Channel { get; private set; }
        [field: SerializeField] public Image FillImage { get; private set; }
    }
}