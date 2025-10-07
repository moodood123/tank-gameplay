using TMPro;
using UnityEngine;

public class LevelSelectionButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _levelTitleText;
    
    private LevelData _data;
    private bool _isSelected = false;
    
    public void Setup(LevelData data)
    {
        _data = data;

        _levelTitleText.text = _data.LevelTitle;
    }

    public void SelectLevel()
    {
        _isSelected = !_isSelected;
        LevelManager.SetCurrentLevelData(_isSelected ? _data : null);
    }
}
