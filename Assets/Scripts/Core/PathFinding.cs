using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinding
{
    private readonly GridManager _gridManager;
    private const int InvalidIndex = -1;
    private const int MovementCost = 1;
    private readonly Vector2Int[] _directions = 
    {
        new Vector2Int(0, 1), 
        new Vector2Int(0, -1),
        new Vector2Int(1, 0), 
        new Vector2Int(-1, 0)  
    };

    public Pathfinding(GridManager gridManager)
    {
        _gridManager = gridManager;
    }

    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int target)
    {
        var openList = new List<Vector2Int> { start };
        var closedSet = new HashSet<Vector2Int>();

        var cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        var gScore = new Dictionary<Vector2Int, int> { [start] = 0 };
        var fScore = new Dictionary<Vector2Int, int> { [start] = Heuristic(start, target) };

        while (openList.Count > 0)
        {
            Vector2Int current = openList.OrderBy(n => fScore.ContainsKey(n) ? fScore[n] : int.MaxValue).First();

            if (current == target)
                return ReconstructPath(cameFrom, current);

            openList.Remove(current);
            closedSet.Add(current);

            foreach (Vector2Int neighbor in GetNeighbors(current))
            {
                if (closedSet.Contains(neighbor) || _gridManager.IsCellOccupied(neighbor))
                    continue;

                int tentativeGScore = gScore[current] + MovementCost;

                if (!openList.Contains(neighbor))
                    openList.Add(neighbor);
                else if (tentativeGScore >= gScore[neighbor])
                    continue;

                cameFrom[neighbor] = current;
                gScore[neighbor] = tentativeGScore;
                fScore[neighbor] = gScore[neighbor] + Heuristic(neighbor, target);
            }
        }

        return null;
    }

    private List<Vector2Int> GetNeighbors(Vector2Int current)
    {
        var neighbors = new List<Vector2Int>();

        foreach (var dir in _directions)
        {
            Vector2Int neighbor = current + dir;
            if (IsValidGridPosition(neighbor))
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    private bool IsValidGridPosition(Vector2Int position)
    {
        return position.x >= 0 && position.x < _gridManager.SizeX && 
               position.y >= 0 && position.y < _gridManager.SizeY;
    }

    private int Heuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current)
    {
        var path = new List<Vector2Int> { current };

        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Add(current);
        }

        path.Reverse();
        path.RemoveAt(0);
        return path;
    }

    public Vector2Int GetClosestTargetInTopRow(Vector2Int currentPosition)
    {
        int topRow = _gridManager.SizeY - 1;
        Vector2Int closestTarget = new Vector2Int(InvalidIndex, InvalidIndex);
        int shortestPathCost = int.MaxValue;

        for (int x = 0; x < _gridManager.SizeX; x++)
        {
            Vector2Int target = new Vector2Int(x, topRow);

            if (_gridManager.IsCellOccupied(target))
                continue;

            List<Vector2Int> path = FindPath(currentPosition, target);
            if (path != null && path.Count < shortestPathCost)
            {
                shortestPathCost = path.Count;
                closestTarget = target;
            }
        }

        return closestTarget;
    }
}
