using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    [SerializeField] private StartPanel _startPanel;
    [SerializeField] private FailPanel _failPanel;
    [SerializeField] private SuccessfulPanel _successfulPanel;
    [SerializeField] private GamePlayPanel _gamePlayPanel;

    public GamePlayPanel GamePlayPanel => _gamePlayPanel;

    public void Initialize(GameManager gameManager)
    {
        _gamePlayPanel.Initialize(gameManager);
    }
    
    private void OnEnable()
    {
        EventManager.Subscribe(GameEvents.OnStartLevel, StartLevel);
        EventManager.Subscribe(GameEvents.OnLevelFail, FailLevel);
        EventManager.Subscribe(GameEvents.OnLevelSuccessful, SuccessfulLevel);
        EventManager.Subscribe(GameEvents.OnNewLevelLoad, OpenStartPanel);
    }

    private void OnDisable()
    {
        EventManager.Unsubscribe(GameEvents.OnStartLevel, StartLevel);
        EventManager.Unsubscribe(GameEvents.OnLevelFail, FailLevel);
        EventManager.Unsubscribe(GameEvents.OnLevelSuccessful, SuccessfulLevel);
        EventManager.Unsubscribe(GameEvents.OnNewLevelLoad, OpenStartPanel);
    }

    private void StartLevel()
    {
        CloseAllPanels();
        _gamePlayPanel.gameObject.SetActive(true);
        _gamePlayPanel.SetLevelText();
    }
    private void OpenStartPanel()
    {
        CloseAllPanels();
        _startPanel.gameObject.SetActive(true);
    }

    private void FailLevel()
    {
        CloseAllPanels();
        _failPanel.gameObject.SetActive(true);
    }

    private void SuccessfulLevel()
    {
        CloseAllPanels();
        _successfulPanel.gameObject.SetActive(true);
    }

    private void CloseAllPanels()
    {
        _startPanel.gameObject.SetActive(false);
        _failPanel.gameObject.SetActive(false);
        _successfulPanel.gameObject.SetActive(false);
        _gamePlayPanel.gameObject.SetActive(false);
    }
}
