using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectionMenu : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private List<LevelData> _availableLevels = new List<LevelData>();

    [Header("References")] 
    [SerializeField] private RectTransform _selectionGroup;
    [SerializeField] private GameObject _levelSelectionPrefab;
    [SerializeField] private Scrollbar _scrollbar;
    [SerializeField] private RectTransform _groupMask;

    [SerializeField] private float _selectionButtonHeight;
    
    private float _maxValue;

    private void OnEnable()
    {
        _scrollbar.onValueChanged.AddListener(OnScroll);
    }

    private void OnDisable()
    {
        _scrollbar.onValueChanged.RemoveListener(OnScroll);
    }
    
    private void Start()
    {
        SetupMenu();
    }

    private void OnScroll(float value)
    {
        _selectionGroup.anchoredPosition = new Vector2(0f, value * _maxValue);
    }
    
    private void SetupMenu()
    {
        int dataCount = 0;
        
        // Add selection panel
        foreach (LevelData level in _availableLevels)
        {
            GameObject go = Instantiate(_levelSelectionPrefab, _selectionGroup);

            if (go.TryGetComponent(out LevelSelectionButton selectionButton))
            {
                go.SetActive(true);
                dataCount++;
                selectionButton.Setup(level);
            }
            else
            {
                Debug.Log("Invalid prefab instantiated");
                Destroy(go);
            }
        }
        
        // Setup scroll bar
        _maxValue = dataCount * _selectionButtonHeight - _groupMask.rect.height + 5f;

    }
}
