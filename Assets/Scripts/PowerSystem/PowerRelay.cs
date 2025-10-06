using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class PowerRelay : MonoBehaviour
{
    [SerializeField] private List<PowerSetting> _initialSettings = new List<PowerSetting>();
    
    public Dictionary<PowerChannel, float> PowerRatios { get; private set; } = new Dictionary<PowerChannel, float>();

    public List<PowerSetting> CurrentSettings { get; private set; } = new List<PowerSetting>();

    private void Start()
    {
        ArrangeRelay(_initialSettings);
    }
    
    private void ArrangeRelay(List<PowerSetting> settings)
    {
        foreach (PowerSetting setting in CurrentSettings) Unsubscribe(setting);
        CurrentSettings = new List<PowerSetting>(settings);
        
        PowerRatios.Clear();
        foreach (PowerSetting setting in CurrentSettings)
        {
            PowerRatios.Add(setting.Channel, 0f);
            Subscribe(setting);
        }
    }

    private void Subscribe(PowerSetting setting)
    {
        setting.Subscribe();
        setting.onPowerChangedRelay += SetValue;
    }

    private void Unsubscribe(PowerSetting setting)
    {
        setting.Unsubscribe();
        setting.onPowerChangedRelay -= SetValue;
    }

    private void SetValue(PowerChannel channel, float value)
    {
        if (PowerRatios.ContainsKey(channel)) PowerRatios[channel] = value;
    }
}

[Serializable]
public class PowerSetting
{
    [field: SerializeField] public PowerChannel Channel { get; set; }
    [field: SerializeField] public PowerGenerator Generator { get; private set; }

    public delegate void OnPowerChangedRelay(PowerChannel channel, float powerRatio);
    public event OnPowerChangedRelay onPowerChangedRelay;

    public void Subscribe() { if (Generator) Generator.onPowerChanged += OnPowerChanged; }
    public void Unsubscribe() { if (Generator) Generator.onPowerChanged -= OnPowerChanged; }
    public void OnPowerChanged(float powerRatio) => onPowerChangedRelay?.Invoke(Channel, powerRatio);
}

public enum PowerChannel
{
    Turret,
    Engine,
    Lighting
}