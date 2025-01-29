using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public enum GameStatus
    {
        None,
        StartScreen,
        Gameplay,
        EndScreen,
    }
    
    [SerializeField] private LevelManager _levelManager;
    [SerializeField] private MatchManager _matchManager;
    [SerializeField] private ObjectPoolManager _objectPoolManager;
    [SerializeField] private UiManager _uiManager;
    [SerializeField] private ColorMaterialSelector _colorMaterialSelector;
    
    private int _currentLevelIndex = 1;

    private GameStatus _currentStatus = GameStatus.None;
    public GameStatus CurrentStatus => _currentStatus;
    public int CurrentLevelIndex => _currentLevelIndex;
    void Awake()
    {
        _currentLevelIndex = GetLevelIndex();
        _colorMaterialSelector.Initialize();
        _levelManager.Initialize(this, _objectPoolManager, _matchManager, _uiManager);
        _levelManager.LoadLevel(_currentLevelIndex);
        _uiManager.Initialize(this);
    }
    
    private void OnEnable()
    {
        EventManager.Subscribe(GameEvents.OnStartLevel, StartLevel);
        EventManager.Subscribe(GameEvents.OnLevelFail, FailLevel);
        //EventManager.Subscribe(GameEvents.OnLevelFailAndNotMove, FailLevelas);
        EventManager.Subscribe(GameEvents.OnLevelSuccessful, SuccessfulLevel);
        EventManager.Subscribe(GameEvents.OnNewLevelLoad, LoadLevel);
    }
    
    private void OnDisable()
    {
        EventManager.Unsubscribe(GameEvents.OnStartLevel, StartLevel);
        EventManager.Unsubscribe(GameEvents.OnLevelFail, FailLevel);
        //EventManager.Unsubscribe(GameEvents.OnLevelFailAndNotMove, FailLevelas);
        EventManager.Unsubscribe(GameEvents.OnLevelSuccessful, SuccessfulLevel);
        EventManager.Unsubscribe(GameEvents.OnNewLevelLoad, LoadLevel);
    }

    private void StartLevel()
    {
        _currentStatus = GameStatus.Gameplay;
    }
    private void FailLevel()
    {
        _currentStatus = GameStatus.EndScreen;
    }    
    // private void FailLevelas()
    // {
    //     _currentStatus = GameStatus.None;
    // } 
    private void SuccessfulLevel()
    {
        _currentStatus = GameStatus.EndScreen;
        _currentLevelIndex++;
        SaveLevel(_currentLevelIndex);
    }

    public void LoadasdLevel()
    {
        _currentLevelIndex++;
        _levelManager.LoadLevel(_currentLevelIndex);

        // Yeni seviyeyi kaydet
        SaveLevel(_currentLevelIndex);
    }
    public void LoadLevel()
    {
        _levelManager.LoadLevel(_currentLevelIndex);
    }

    public void SaveLevel(int levelIndex)
    {
        PlayerPrefs.SetInt("CurrentLevelIndex", levelIndex);
        PlayerPrefs.Save();
    }

    public int GetLevelIndex()
    {
        return PlayerPrefs.GetInt("CurrentLevelIndex", 1); // Varsayılan seviye 0
    }
}