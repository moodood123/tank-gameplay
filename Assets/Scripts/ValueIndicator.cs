using TMPro;
using UnityEngine;

public class ValueIndicator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    
    private string _value;

    public void SetValue(string value)
    {
        _value = value;
        _text.text = _value;
    }
}
