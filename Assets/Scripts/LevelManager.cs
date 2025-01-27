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
    
    public Levels levelsData; 

    private ObjectPoolManager _poolManager;
    private MatchManager _matchManager;
    private Pathfinding pathfinding; 
    private List<Human> humans = new List<Human>();
    private List<SingleGridController> gridCells = new List<SingleGridController>();

    public void Initialize(ObjectPoolManager poolManager, MatchManager matchManager)
    {
        _poolManager = poolManager;
        _matchManager = matchManager;
    }
    
    public void LoadLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levelsData.levels.Count)
        {
            Debug.LogError("Geçersiz seviye indeksi!");
            return;
        }

        ClearCurrentLevel();

        LevelData levelData = levelsData.levels[levelIndex];

        _gridManager.Initialize(levelData, _poolManager);
        pathfinding = new Pathfinding(_gridManager);
        _busStopManager.Initialize(_poolManager);
        _busController.Initialize(levelData, _poolManager);
        _humanManager.Initialize(_gridManager, _poolManager, pathfinding, _matchManager);
        _matchManager.Initialize(_busController, _busStopManager);
        _inputManager.Initialize(_humanManager);
        foreach (Vector2Int startPosition in levelData.humanStartPositions)
        {
            _humanManager.SpawnHuman(startPosition);
        }
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
    }
}
