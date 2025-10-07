using System.Collections;
using TMPro;
using UnityEngine;

public class LobbyMenu : MonoBehaviour
{
    public RelayManager relayManager;
    public TMP_InputField joinCodeInputField;

    public void HostGame()
    {
        StartCoroutine(HostRoutine());
    }

    public void JoinGame()
    {
        StartCoroutine(JoinRoutine(joinCodeInputField.text));
    }

    private IEnumerator HostRoutine()
    {
        yield return relayManager.StartHostAsync(6).AsCoroutine();
    }

    private IEnumerator JoinRoutine(string joinCode)
    {
        yield return relayManager.StartClientAsync(joinCode).AsCoroutine();
    }
}
