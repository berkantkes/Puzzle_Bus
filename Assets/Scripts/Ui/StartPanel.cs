using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartPanel : MonoBehaviour
{
    [SerializeField] private Button _tapToStartButton;

    private void OnEnable()
    {
        _tapToStartButton.onClick.AddListener(ClickStart);
    }

    private void OnDisable()
    {
        _tapToStartButton.onClick.RemoveListener(ClickStart);
    }

    private void ClickStart()
    {
        gameObject.SetActive(false);
        EventManager.Execute(GameEvents.OnStartLevel);
    }
}
