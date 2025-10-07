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

    [SerializeField] private GameObject _presessionPanel;
    [SerializeField] private GameObject _sessionPanel;

    private void OnEnable()
    {
        _joinCodeInputField.onValueChanged.AddListener(OnInputChanged);
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
        NetworkManager.Singleton.OnServerStopped += OnServerStopped;
        NetworkManager.Singleton.OnClientStarted += OnClientStarted;
        NetworkManager.Singleton.OnServerStopped += OnClientStopped;
    }

    private void OnDisable()
    {
        _joinCodeInputField.onValueChanged.RemoveListener(OnInputChanged);

        if (NetworkManager.Singleton)
        {
            NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
            NetworkManager.Singleton.OnServerStopped -= OnServerStopped;
            NetworkManager.Singleton.OnClientStarted -= OnClientStarted;
            NetworkManager.Singleton.OnClientStopped -= OnClientStopped;
        }
    }

    private void Start()
    {
        _joinCodeButton.interactable = false;
    }
    
    private void OnServerStarted()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            _presessionPanel.SetActive(false);
            _sessionPanel.SetActive(true);
        }
    }

    private void OnServerStopped(bool _)
    {
        if (NetworkManager.Singleton.IsHost)
        {
            _presessionPanel.SetActive(true);
            _sessionPanel.SetActive(false);
        }
    }

    private void OnClientStarted()
    {
        if (NetworkManager.Singleton.IsClient)
        {
            _presessionPanel.SetActive(false);
            _sessionPanel.SetActive(true);
        }
    }

    private void OnClientStopped(bool _)
    {
        if (NetworkManager.Singleton.IsClient)
        {
            _presessionPanel.SetActive(true);
            _sessionPanel.SetActive(false);
        }
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
