using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerController : MonoBehaviour
{
    private GameManager _gameManager;
    private UiManager _uiManager;
    private float _baseTime;
    private bool _isTimeCount = true;

    public void Initialize(GameManager gameManager, float baseTime, UiManager uiManager)
    {
        _gameManager = gameManager;
        _uiManager = uiManager;
        _baseTime = baseTime;
        _isTimeCount = true;
    }
    
    void Update()
    {
        if (_gameManager.CurrentStatus == GameManager.GameStatus.Gameplay && _isTimeCount)
        {
            if (_baseTime < 0)
            {
                _isTimeCount = false;
                EventManager.Execute(GameEvents.OnLevelFail);
            }
                
            _baseTime -= Time.deltaTime;
            _uiManager.GamePlayPanel.SetTimerText(_baseTime);
        }
    }
}
