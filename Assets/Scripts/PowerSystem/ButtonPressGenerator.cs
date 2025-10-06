using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonPressGenerator : PowerGenerator
{
    [SerializeField] private float _powerIncreaseOnPress;
    [SerializeField] private float _interfaceUpdateInterval;

    [SerializeField] private TextMeshProUGUI _powerText;
    [SerializeField] private Image _powerImage;
    
    protected override void Start()
    {
        base.Start();
        StartCoroutine(UpdateLoop());
    }
    
    private IEnumerator UpdateLoop()
    {
        while (true)
        {
            _powerText.text = CurrentPowerRatio.ToString("0.00");
            _powerImage.fillAmount = CurrentPowerRatio;
            
            if (_interfaceUpdateInterval > 0f) yield return new WaitForSeconds(_interfaceUpdateInterval);
            else yield return null;
        }
    }
    
    public void OnPress()
    {
        IncrementPower(_powerIncreaseOnPress);
    }
}
