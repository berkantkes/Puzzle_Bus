using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class HumanManager : MonoBehaviour
{
    private List<Human> _humans = new List<Human>();
    private GridManager _gridManager;
    private ObjectPoolManager _poolManager;
    private Pathfinding _pathfinding;
    private MatchManager _matchManager;

    private const float SelectionRadius = 0.5f; 
    private const float TargetDistanceThreshold = 0.1f; 
    private static readonly Vector2Int InvalidGridPosition = new Vector2Int(-1, -1);

    public void Initialize(GridManager gridManager, ObjectPoolManager poolManager, Pathfinding pathfinding, MatchManager matchManager, LevelData levelData)
    {
        _gridManager = gridManager;
        _poolManager = poolManager;
        _pathfinding = pathfinding;
        _matchManager = matchManager;

        foreach (HumanData startPosition in levelData.HumanDatas)
        {
            SpawnHuman(startPosition);
        }

        UpdateHumanOutlines();
    }

    public void SpawnHuman(HumanData position)
    {
        Vector3 worldPosition = _gridManager.GetWorldPosition(position.HumanStartPosition);
        Human human = _poolManager.GetFromPool<Human>(ObjectType.Human, worldPosition, Quaternion.Euler(0, 180, 0));
        human.Initialize(position.HumanColorType);
        human.transform.SetParent(transform);

        _humans.Add(human);
        _gridManager.SetCellOccupied(position.HumanStartPosition, true);
    }

    public List<Human> GetHumans() => _humans;

    public void MoveHuman(Vector3 worldPosition)
    {
        foreach (Human human in _humans)
        {
            if (Vector3.Distance(human.transform.position, worldPosition) < SelectionRadius)
            {
                HandleHumanMovement(human);
            }
        }

        UpdateHumanOutlines();
    }

    private void HandleHumanMovement(Human human)
    {
        Vector2Int currentPosition = _gridManager.GetGridPosition(human.transform.position);
        int topRowIndex = _gridManager.SizeY - 1;

        if (currentPosition.y != topRowIndex)
        {
            Vector2Int targetPosition = _pathfinding.GetClosestTargetInTopRow(currentPosition);

            if (targetPosition != InvalidGridPosition)
            {
                List<Vector2Int> path = _pathfinding.FindPath(currentPosition, targetPosition);

                if (path != null)
                {
                    human.UpdateMaterial(false);
                    human.MoveAlongPath(path, _gridManager);
                    _gridManager.SetCellOccupied(currentPosition, false);
                    StartCoroutine(CheckIfHumanReachedTopRow(human, targetPosition));
                }
            }
        }
        else
        {
            human.UpdateMaterial(false);
            _gridManager.SetCellOccupied(currentPosition, false);
            _matchManager.MoveToBusStop(human, _gridManager);
        }
    }

    private void UpdateHumanOutlines()
    {
        int topRowIndex = _gridManager.SizeY - 1;

        foreach (Human human in _humans)
        {
            Vector2Int currentPosition = _gridManager.GetGridPosition(human.transform.position);

            if (currentPosition.y != topRowIndex)
            {
                Vector2Int targetPosition = _pathfinding.GetClosestTargetInTopRow(currentPosition);

                if (targetPosition != InvalidGridPosition)
                {
                    List<Vector2Int> path = _pathfinding.FindPath(currentPosition, targetPosition);
                    human.UpdateMaterial(path != null);
                }
            }
            else
            {
                human.UpdateMaterial(true);
            }
        }
    }

    private IEnumerator CheckIfHumanReachedTopRow(Human human, Vector2Int position)
    {
        while (Vector3.Distance(human.transform.position, _gridManager.GetWorldPosition(position)) > TargetDistanceThreshold)
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
