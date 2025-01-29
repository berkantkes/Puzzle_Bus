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
        MatchManager matchManager, LevelData levelData)
    {
        _gridManager = gridManager;
        _poolManager = poolManager;
        _pathfinding = pathfinding;
        _matchManager = matchManager;
        
        foreach (HumanData startPosition in levelData.HumanDatas)
        {
            SpawnHuman(startPosition);
        }
        
        CheckPathForOutline();
    }
    
    public void SpawnHuman(HumanData position)
    {
        Vector3 worldPosition = _gridManager.GetWorldPosition(position.HumanStartPosition);
        Human human = _poolManager.GetFromPool<Human>(ObjectType.Human, worldPosition, Quaternion.identity);
        human.Initialize(position.HumanColorType);
        human.transform.SetParent(transform);
        human.transform.rotation = Quaternion.Euler(0, 180, 0);
        //Human human = humanObj.GetComponent<Human>();

        _humans.Add(human);
        _gridManager.SetCellOccupied(position.HumanStartPosition, true);
    }

    public List<Human> GetHumans()
    {
        return _humans;
    }
    
    public void MoveHuman(Vector3 worldPosition)
    {
        foreach (Human human in _humans)
        {
            if (Vector3.Distance(human.transform.position, worldPosition) < 0.5f)
            {
                Vector2Int currentPosition = _gridManager.GetGridPosition(human.transform.position);

                if (currentPosition.y != _gridManager.SizeX - 1)
                {
                    Vector2Int targetPosition = _pathfinding.GetClosestTargetInTopRow(currentPosition);

                    if (targetPosition != new Vector2Int(-1, -1))
                    {
                        List<Vector2Int> path = _pathfinding.FindPath(currentPosition, targetPosition);
                        
                        if (path != null)
                        {
                            human.UpdateMaterial(false);
                            human.MoveAlongPath(path, _gridManager);
                            _gridManager.SetCellOccupied(currentPosition, false);
                            
                            StartCoroutine(CheckIfAtTopRow(human, targetPosition));
                            
                        }
                    }
                }
                else
                {
                    human.UpdateMaterial(false);
                    _matchManager.MoveToBusStop(human, _gridManager);
                }
            }
        }

        CheckPathForOutline();
    }

    private void CheckPathForOutline()
    {
        foreach (Human human in _humans)
        {
            Vector2Int currentPosition = _gridManager.GetGridPosition(human.transform.position);

            Debug.Log(currentPosition);
            
            if (currentPosition.y != _gridManager.SizeY - 1)
            {
                Vector2Int targetPosition = _pathfinding.GetClosestTargetInTopRow(currentPosition);

                if (targetPosition != new Vector2Int(-1, -1))
                {
                    List<Vector2Int> path = _pathfinding.FindPath(currentPosition, targetPosition);
                        
                    human.UpdateMaterial(path != null);
                    // if (path != null)
                    // {
                    //     //outline işlemi       
                    // }
                }
            }
            else
            {
                human.UpdateMaterial(true);
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
    
    public void ClearHumans()
    {
        foreach (Human human in _humans)
        {
            if (human != null)
            {
                human.ClearHuman();
                _poolManager.ReturnToPool(ObjectType.Human, human.gameObject);
            }
        }

        _humans.Clear();
    }
}
