using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] private RelayManager _relayManager;
    [SerializeField] private TMP_InputField _joinCodeInputField;
    [SerializeField] private Button _joinCodeButton;
    [SerializeField] private Button _startGameButton;

    [SerializeField] private GameObject _PresessionPanel;
    [SerializeField] private GameObject _SessionPanel;

    private void OnEnable()
    {
        _joinCodeInputField.onValueChanged.AddListener(OnInputChanged);
    }

    private void OnDisable()
    {
        _joinCodeInputField.onValueChanged.RemoveListener(OnInputChanged);
    }

    private void Start()
    {
        _joinCodeButton.interactable = false;
    }

    private void OnInputChanged(string code)
    {
        if (string.IsNullOrEmpty(code)) _joinCodeButton.interactable = false;
        else _joinCodeButton.interactable = true;
    }
    
    public void HostGame()
    {
        StartCoroutine(HostRoutine());
    }

    public void JoinGame()
    {
        StartCoroutine(JoinRoutine(_joinCodeInputField.text));
    }

    private IEnumerator HostRoutine()
    {
        yield return _relayManager.StartHostAsync(6).AsCoroutine();
    }

    private IEnumerator JoinRoutine(string joinCode)
    {
        yield return _relayManager.StartClientAsync(joinCode).AsCoroutine();
    }
}
