using TMPro;
using UnityEngine;

public class PlayerTile : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _idText;
    
    public void Setup(ulong playerId)
    {
        _idText.text = playerId.ToString();
    }
}
