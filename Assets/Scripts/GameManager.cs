using UnityEngine;

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

    private int _currentLevelIndex;
    private int _defaultLevelIndex = 1;
    private GameStatus _currentStatus = GameStatus.None;

    public GameStatus CurrentStatus => _currentStatus;
    public int CurrentLevelIndex => _currentLevelIndex;

    private void Awake()
    {
        InitializeGame();
    }

    private void OnEnable()
    {
        SubscribeEvents();
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    private void InitializeGame()
    {
        _currentLevelIndex = LoadSavedLevel();
        _colorMaterialSelector.Initialize();
        _levelManager.Initialize(this, _objectPoolManager, _matchManager, _uiManager);
        _levelManager.LoadLevel(_currentLevelIndex);
        _uiManager.Initialize(this);
    }

    private void SubscribeEvents()
    {
        EventManager.Subscribe(GameEvents.OnStartLevel, StartLevel);
        EventManager.Subscribe(GameEvents.OnLevelFail, FailLevel);
        EventManager.Subscribe(GameEvents.OnLevelSuccessful, SuccessfulLevel);
        EventManager.Subscribe(GameEvents.OnNewLevelLoad, LoadNextLevel);
    }

    private void UnsubscribeEvents()
    {
        EventManager.Unsubscribe(GameEvents.OnStartLevel, StartLevel);
        EventManager.Unsubscribe(GameEvents.OnLevelFail, FailLevel);
        EventManager.Unsubscribe(GameEvents.OnLevelSuccessful, SuccessfulLevel);
        EventManager.Unsubscribe(GameEvents.OnNewLevelLoad, LoadNextLevel);
    }

    private void StartLevel()
    {
        _currentStatus = GameStatus.Gameplay;
    }

    private void FailLevel()
    {
        _currentStatus = GameStatus.EndScreen;
    }

    private void SuccessfulLevel()
    {
        _currentStatus = GameStatus.EndScreen;
        _currentLevelIndex++;
        SaveCurrentLevel();
    }

    private void LoadNextLevel()
    {
        //_currentLevelIndex++;
        _levelManager.LoadLevel(_currentLevelIndex);
        SaveCurrentLevel();
    }

    private void SaveCurrentLevel()
    {
        PlayerPrefs.SetInt("CurrentLevelIndex", _currentLevelIndex);
        PlayerPrefs.Save();
    }

    private int LoadSavedLevel()
    {
        return PlayerPrefs.GetInt("CurrentLevelIndex", _defaultLevelIndex);
    }
}
