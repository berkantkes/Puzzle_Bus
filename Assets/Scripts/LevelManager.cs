using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private BusStopManager _busStopManager;
    [SerializeField] private BusController _busController; 
    [SerializeField] private HumanManager _humanManager;
    [SerializeField] private GridManager _gridManager; 
    [SerializeField] private InputManager _inputManager;
    [SerializeField] private TimerController _timerController;
    
    public LevelData levelsData;

    private GameManager _gameManager;
    private ObjectPoolManager _poolManager;
    private MatchManager _matchManager;
    private UiManager _uiManager;
    private Pathfinding pathfinding; 
    private List<Human> humans = new List<Human>();
    private List<SingleGridController> gridCells = new List<SingleGridController>();

    public void Initialize(GameManager gameManager, ObjectPoolManager poolManager, MatchManager matchManager, UiManager uiManager)
    {
        _gameManager = gameManager;
        _poolManager = poolManager;
        _matchManager = matchManager;
        _uiManager = uiManager;
    }
    
    private void OnEnable()
    {
    }
    
    private void OnDisable()
    {
    }
    
    public void LoadLevel(int levelIndex)
    {
        // if (levelIndex < 0 || levelIndex >= levelsData.levels.Count)
        // {
        //     Debug.LogError("Geçersiz seviye indeksi!");
        //     return;
        // }

        ClearCurrentLevel();

        LevelData levelData = levelsData;

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
}
