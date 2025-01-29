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

        // Eğer istenen level yoksa, önceki levellerden rastgele bir tane seç
        if (levelData == null)
        {
            Debug.LogWarning($"Level {levelIndex} bulunamadı! Önceki levellerden rastgele bir tane seçiliyor...");

            List<LevelData> availableLevels = GetAvailableLevels(levelIndex);

            if (availableLevels.Count > 0)
            {
                levelData = availableLevels[Random.Range(0, availableLevels.Count)];
                Debug.Log($"Seçilen rastgele level: {levelData.levelNumber}");
            }
            else
            {
                Debug.LogError("Mevcut hiçbir level bulunamadı!");
                return;
            }
        }

        // Seçilen leveli yükle
        _gridManager.Initialize(levelData, _poolManager);
        _pathfinding = new Pathfinding(_gridManager);
        _busStopManager.Initialize(_poolManager);
        _busController.Initialize(levelData, _poolManager);
        _humanManager.Initialize(_gridManager, _poolManager, _pathfinding, _matchManager, levelData);
        _matchManager.Initialize(_busController, _busStopManager);
        _inputManager.Initialize(_gameManager, _humanManager);
        _timerController.Initialize(_gameManager, levelData.time, _uiManager);
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

        if (levels == null || levels.Length == 0)
        {
            Debug.LogError($"Resources klasöründen '{LevelDataPath}' altındaki Levels varlıkları yüklenemedi!");
            return new List<LevelData>();
        }

        return levels.Select(level => level.GetData()).OrderBy(data => data.levelNumber).ToList();
    }

    public LevelData GetLevelData(int levelNumber)
    {
        return _levelDatas.FirstOrDefault(data => data.levelNumber == levelNumber);
    }
}
