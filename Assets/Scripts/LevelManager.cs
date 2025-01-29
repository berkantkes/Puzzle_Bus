using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Serialization;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private BusStopManager _busStopManager;
    [SerializeField] private BusController _busController; 
    [SerializeField] private HumanManager _humanManager;
    [SerializeField] private GridManager _gridManager; 
    [SerializeField] private InputManager _inputManager;
    [SerializeField] private TimerController _timerController;
    
    private const string PathOfData = "Datas/Levels";

    private List<LevelData> _datas = new List<LevelData>();
    private GameManager _gameManager;
    private ObjectPoolManager _poolManager;
    private MatchManager _matchManager;
    private UiManager _uiManager;
    private Pathfinding pathfinding; 
    private List<Human> humans = new List<Human>();
    private List<SingleGridController> gridCells = new List<SingleGridController>();

    public void Initialize(GameManager gameManager, ObjectPoolManager poolManager, MatchManager matchManager, UiManager uiManager)
    {
        _datas = GetDatas();
        _gameManager = gameManager;
        _poolManager = poolManager;
        _matchManager = matchManager;
        _uiManager = uiManager;
    }
    
    public void LoadLevel(int levelIndex)
    {
        ClearCurrentLevel();

        LevelData levelData = GetLevelData(levelIndex);

        _gridManager.Initialize(levelData, _poolManager);
        pathfinding = new Pathfinding(_gridManager);
        _busStopManager.Initialize(_poolManager);
        _busController.Initialize(levelData, _poolManager);
        _humanManager.Initialize(_gridManager, _poolManager, pathfinding, _matchManager, levelData);
        _matchManager.Initialize(_busController, _busStopManager); //gamemanager init
        _inputManager.Initialize(_gameManager, _humanManager); // gamemanager init
        _timerController.Initialize(_gameManager, levelData.time, _uiManager); // gamemanager init
    }

    private void ClearCurrentLevel()
    {
        foreach (Human human in humans)
        {
            _poolManager.ReturnToPool(ObjectType.Human, human.gameObject);
        }
        humans.Clear();

        foreach (SingleGridController cell in gridCells)
        {
            _poolManager.ReturnToPool(ObjectType.GridCell, cell.gameObject);
        }
        gridCells.Clear();
        _gridManager.ClearGrid();
        pathfinding = null;
        _busStopManager.ClearBusStops();
        _busController.ClearBuses();
        _humanManager.ClearHumans();
    }
    
    public List<LevelData> GetDatas()
    {
        var levels = Resources.LoadAll<Levels>(PathOfData);

        if (levels == null || levels.Length == 0)
        {
            Debug.LogError($"Resources klasöründen Datas/Levels altındaki Levels varlıkları yüklenemedi!");
            return new List<LevelData>();
        }

        return new List<LevelData>(levels.Select(item => item.GetData())
            .OrderBy(data => data.levelNumber).ToList());
    }
    public LevelData GetLevelData(int id)
    {
        return _datas.FirstOrDefault(data => data.levelNumber == id);
    }
}
