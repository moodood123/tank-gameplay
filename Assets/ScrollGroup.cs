using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollGroup : MonoBehaviour
{
    [Header("UI Elements")] 
    [SerializeField] private Scrollbar _scrollbar;
    [SerializeField] private VerticalLayoutGroup _layoutGroup;
    [SerializeField] private RectTransform _groupRect;

    private List<RectTransform> _currentElements = new List<RectTransform>();
    
    private float _maxValue;
    
    private void OnEnable()
    {
        _scrollbar.onValueChanged.AddListener(OnScroll);
    }

    private void OnDisable()
    {
        _scrollbar.onValueChanged.RemoveListener(OnScroll);
    }

    private void OnScroll(float value)
    {
        _groupRect.anchoredPosition = new Vector2(0f, value * _maxValue);
    }
    
    public void SetupMenu(List<RectTransform> elements)
    {
        foreach (RectTransform element in _currentElements)
        {
            Destroy(element.gameObject);
        }
        
        _currentElements = elements;
        _maxValue = _layoutGroup.padding.bottom + _layoutGroup.padding.top + _layoutGroup.spacing * elements.Count;
        
        foreach (RectTransform element in elements)
        {
            element.SetParent(_layoutGroup.transform);
            element.localScale = Vector3.one;
            _maxValue += element.sizeDelta.y;
        }
    }
}
