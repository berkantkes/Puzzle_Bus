using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SuccessfulPanel : MonoBehaviour
{
    [SerializeField] private Button _nextLevelButton;

    private void OnEnable()
    {
        _nextLevelButton.onClick.AddListener(ClickNextLevel);
    }

    private void OnDisable()
    {
        _nextLevelButton.onClick.RemoveListener(ClickNextLevel);
    }

    private void ClickNextLevel()
    {
        gameObject.SetActive(false);
        EventManager.Execute(GameEvents.OnNewLevelLoad);
    }
}
