using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GamePlayPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text _levelText;
    [SerializeField] private TMP_Text _timerText;

    private GameManager _gameManager;

    public void Initialize(GameManager gameManager)
    {
        _gameManager = gameManager;
    }
    
    public void SetTimerText(float timer)
    {
        _timerText.SetText(((int)timer).ToString());
    }

    public void SetLevelText()
    {
        _levelText.SetText("Level " + _gameManager.CurrentLevelIndex);
    }
    
}
