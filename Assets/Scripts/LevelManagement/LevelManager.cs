using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private BusStopManager _busStopManager;
    [SerializeField] private BusController _busController;
    [SerializeField] private HumanManager _humanManager;
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private InputManager _inputManager;
    [SerializeField] private TimerController _timerController;

    private const string LevelDataPath = "Datas/Levels";
    private const string RandomLevelKey = "RandomLevelIndex";

    private List<LevelData> _levelDatas = new List<LevelData>();
    private GameManager _gameManager;
    private ObjectPoolManager _poolManager;
    private MatchManager _matchManager;
    private UiManager _uiManager;
    private Pathfinding _pathfinding;

    private List<Human> _activeHumans = new List<Human>();
    private List<SingleGridController> _activeGridCells = new List<SingleGridController>();

    public void Initialize(GameManager gameManager, ObjectPoolManager poolManager, MatchManager matchManager, UiManager uiManager)
    {
        _levelDatas = LoadAllLevelData();
        _gameManager = gameManager;
        _poolManager = poolManager;
        _matchManager = matchManager;
        _uiManager = uiManager;
    }

    public void LoadLevel(int levelIndex)
    {
        ClearCurrentLevel();
        LevelData levelData = GetLevelData(levelIndex);

        if (levelData == null)
        {
            levelData = GetOrAssignRandomLevel(levelIndex);
            if (levelData == null) return; 
        }

        _gridManager.Initialize(levelData, _poolManager);
        _pathfinding = new Pathfinding(_gridManager);
        _busStopManager.Initialize(_poolManager);
        _busController.Initialize(levelData, _poolManager);
        _humanManager.Initialize(_gridManager, _poolManager, _pathfinding, _matchManager, levelData);
        _matchManager.Initialize(_busController, _busStopManager);
        _inputManager.Initialize(_gameManager, _humanManager);
        _timerController.Initialize(_gameManager, levelData.time, _uiManager);
    }

    private LevelData GetOrAssignRandomLevel(int maxIndex)
    {
        if (PlayerPrefs.HasKey(RandomLevelKey))
        {
            int savedRandomLevel = PlayerPrefs.GetInt(RandomLevelKey);
            LevelData savedLevel = _levelDatas.FirstOrDefault(data => data.levelNumber == savedRandomLevel);
            if (savedLevel != null) return savedLevel;
        }

        List<LevelData> availableLevels = GetAvailableLevels(maxIndex);
        if (availableLevels.Count > 0)
        {
            LevelData randomLevel = availableLevels[Random.Range(0, availableLevels.Count)];
            PlayerPrefs.SetInt(RandomLevelKey, randomLevel.levelNumber); 
            PlayerPrefs.Save();
            return randomLevel;
        }

        return null; 
    }

    private List<LevelData> GetAvailableLevels(int maxIndex)
    {
        return _levelDatas.Where(data => data.levelNumber < maxIndex).ToList();
    }

    private void ClearCurrentLevel()
    {
        ClearHumans();
        ClearGridCells();
        _gridManager.ClearGrid();
        _pathfinding = null;
        _busStopManager.ClearBusStops();
        _busController.ClearBuses();
        _humanManager.ClearHumans();
    }

    private void ClearHumans()
    {
        foreach (Human human in _activeHumans)
        {
            _poolManager.ReturnToPool(ObjectType.Human, human.gameObject);
        }
        _activeHumans.Clear();
    }

    private void ClearGridCells()
    {
        foreach (SingleGridController cell in _activeGridCells)
        {
            _poolManager.ReturnToPool(ObjectType.GridCell, cell.gameObject);
        }
        _activeGridCells.Clear();
    }

    private List<LevelData> LoadAllLevelData()
    {
        var levels = Resources.LoadAll<Levels>(LevelDataPath);
        if (levels == null || levels.Length == 0) return new List<LevelData>();
        return levels.Select(level => level.GetData()).OrderBy(data => data.levelNumber).ToList();
    }

    public LevelData GetLevelData(int levelNumber)
    {
        return _levelDatas.FirstOrDefault(data => data.levelNumber == levelNumber);
    }
}
