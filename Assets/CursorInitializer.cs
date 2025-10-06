using UnityEngine;

public class CursorInitializer : MonoBehaviour
{
    [SerializeField] private bool _showOnEnable = true;
    
    private void OnEnable()
    {
        Cursor.visible = _showOnEnable;
        Cursor.lockState = _showOnEnable ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
