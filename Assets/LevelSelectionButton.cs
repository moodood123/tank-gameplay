using TMPro;
using UnityEngine;

public class LevelSelectionButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _levelTitleText;
    
    private LevelData _data;
    
    public void Setup(LevelData data)
    {
        _data = data;

        _levelTitleText.text = _data.LevelTitle;
    }

    public void SelectLevel()
    {
        LevelManager.SetCurrentLevelData(_data);
    }
}
