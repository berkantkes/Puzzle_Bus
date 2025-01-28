using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FailPanel : MonoBehaviour
{
    [SerializeField] private Button _retryButton;

    private void OnEnable()
    {
        _retryButton.onClick.AddListener(ClickRetry);
    }

    private void OnDisable()
    {
        _retryButton.onClick.RemoveListener(ClickRetry);
    }

    private void ClickRetry()
    {
        gameObject.SetActive(false);
        EventManager.Execute(GameEvents.OnNewLevelLoad);
    }
}
