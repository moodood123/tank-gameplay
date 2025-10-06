using System.Collections;
using UnityEngine;

public abstract class PowerGenerator : MonoBehaviour
{
    [Header("Defaults")]
    [SerializeField] protected float _initialPowerRatio;
    [SerializeField] protected float _defaultPowerDecay = 0.05f;
    [SerializeField] protected bool _decayOnStart = false;
    
    protected float CurrentPowerRatio { get; private set; }

    protected float _powerDecayRate;
    protected bool _isPowerDecayEnabled;
    
    public delegate void OnPowerChanged(float powerRatio);
    public event OnPowerChanged onPowerChanged;

    protected virtual void Start()
    {
        SetPower(_initialPowerRatio);
        SetPowerDecay(_defaultPowerDecay);
        SetPowerDecayState(_decayOnStart);
        StartCoroutine(PowerDecayLoop());
    }
    
    protected void SetPower(float powerRatio)
    {
        powerRatio = Mathf.Clamp01(powerRatio);
        if (Mathf.Approximately(CurrentPowerRatio, powerRatio)) return;
        
        CurrentPowerRatio = powerRatio;
        onPowerChanged?.Invoke(CurrentPowerRatio);
    }

    protected void IncrementPower(float powerIncrease) => SetPower(CurrentPowerRatio + powerIncrease);

    protected void SetPowerDecayState(bool isEnabled)
    {
        _isPowerDecayEnabled = isEnabled;
    }

    protected void SetPowerDecay(float decayRate)
    {
        _powerDecayRate = decayRate;
    }

    private IEnumerator PowerDecayLoop()
    {
        while (true)
        {
            yield return new WaitUntil(() => _isPowerDecayEnabled);
            SetPower(CurrentPowerRatio - _powerDecayRate * Time.deltaTime);
            yield return null;
        }
    }
}
