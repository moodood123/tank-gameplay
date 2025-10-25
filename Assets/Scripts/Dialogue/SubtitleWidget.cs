using System;
using TMPro;
using UnityEngine;

public class SubtitleWidget : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _subtitleText;
    [SerializeField] private GameObject _backdrop;
    
    private void OnEnable()
    {
        if (DialogueManager.Instance) DialogueManager.Instance.onDisplaySubtitleText += DisplaySubtitles;
    }

    private void OnDisable()
    {
        if (DialogueManager.Instance) DialogueManager.Instance.onDisplaySubtitleText -= DisplaySubtitles;
    }

    private void DisplaySubtitles(string text)
    {
        _subtitleText.text = text;
        _backdrop.SetActive(text != string.Empty);
    }
}
