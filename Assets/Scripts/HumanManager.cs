using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class HumanManager : MonoBehaviour
{
    private List<Human> _humans = new List<Human>();
    private GridManager _gridManager;
    private ObjectPoolManager _poolManager;
    private Pathfinding _pathfinding;
    private MatchManager _matchManager;
    
    public void Initialize(GridManager gridManager, ObjectPoolManager poolManager, Pathfinding pathfinding, 
        MatchManager matchManager)
    {
        _gridManager = gridManager;
        _poolManager = poolManager;
        _pathfinding = pathfinding;
        _matchManager = matchManager;
    }
    
    public void SpawnHuman(Vector2Int position)
    {
        Vector3 worldPosition = _gridManager.GetWorldPosition(position);
        Human human = _poolManager.GetFromPool<Human>(ObjectType.Human, worldPosition, Quaternion.identity);
        human.transform.SetParent(transform);
        human.transform.rotation = Quaternion.Euler(0, 180, 0);
        //Human human = humanObj.GetComponent<Human>();

        _humans.Add(human);
        _gridManager.SetCellOccupied(position, true);
    }

    public List<Human> GetHumans()
    {
        return _humans;
    }
    
    public async void HandleClick(Vector3 worldPosition)
    {
        Debug.Log("HandleClick 1 ");
        foreach (Human human in _humans)
        {
            Debug.Log("HandleClick 2 ");
            if (Vector3.Distance(human.transform.position, worldPosition) < 0.5f)
            {
                Debug.Log("HandleClick 3 ");
                Vector2Int currentPosition = _gridManager.GetGridPosition(human.transform.position);

                if (currentPosition.y != _gridManager.SizeX - 1)
                {
                    Debug.Log("HandleClick 3.1 ");
                    Vector2Int targetPosition = _pathfinding.GetClosestTargetInTopRow(currentPosition);

                    Debug.Log(targetPosition);
                    if (targetPosition != new Vector2Int(-1, -1))
                    {
                        Debug.Log("HandleClick 3.2 ");
                        List<Vector2Int> path = _pathfinding.FindPath(currentPosition, targetPosition);

                        foreach (var pa in path)
                        {
                            Debug.Log(pa);
                        }
                        
                        if (path != null)
                        {
                            Debug.Log("HandleClick 4 ");
                            //StartCoroutine(human.MoveAlongPath(path, _gridManager));
                            human.MoveAlongPath(path, _gridManager);
                            _gridManager.SetCellOccupied(currentPosition, false);
                            
                            StartCoroutine(CheckIfAtTopRow(human, targetPosition));
                            
                            //StartCoroutine(CheckIfAtTopRow(human, targetPosition));
                        }
                    }
                }
                else
                {
                    _matchManager.MoveToBusStop(human, _gridManager);
                }
            }
        }
    }
    
    private IEnumerator CheckIfAtTopRow(Human human, Vector2Int position)
    {
        while (Vector3.Distance(human.transform.position, _gridManager.GetWorldPosition(position)) > 0.1f)
        {
            yield return null;
        }

        _matchManager.MoveToBusStop(human, _gridManager);
    }
}
