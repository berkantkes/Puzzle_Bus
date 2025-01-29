using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinding
{
    private GridManager _gridManager;

    public Pathfinding(GridManager gridManager)
    {
        _gridManager = gridManager;
    }

    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int target)
    {
        List<Vector2Int> openList = new List<Vector2Int> { start };
        HashSet<Vector2Int> closedList = new HashSet<Vector2Int>();

        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        Dictionary<Vector2Int, int> gScore = new Dictionary<Vector2Int, int>();
        Dictionary<Vector2Int, int> fScore = new Dictionary<Vector2Int, int>();

        gScore[start] = 0;
        fScore[start] = Heuristic(start, target);

        while (openList.Count > 0)
        {
            Vector2Int current = openList.OrderBy(n => fScore.ContainsKey(n) ? fScore[n] : int.MaxValue).First();

            if (current == target)
            {
                return ReconstructPath(cameFrom, current);
            }

            openList.Remove(current);
            closedList.Add(current);

            foreach (Vector2Int neighbor in GetNeighbors(current))
            {
                if (closedList.Contains(neighbor) || _gridManager.IsCellOccupied(neighbor)) continue;

                int tentativeGScore = gScore[current] + 1;

                if (!openList.Contains(neighbor))
                    openList.Add(neighbor);
                else if (tentativeGScore >= gScore[neighbor])
                    continue;

                cameFrom[neighbor] = current;
                gScore[neighbor] = tentativeGScore;
                fScore[neighbor] = gScore[neighbor] + Heuristic(neighbor, target);
            }
        }

        return null; // Yol bulunamadı
    }

    private List<Vector2Int> GetNeighbors(Vector2Int current)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        Vector2Int[] directions = {
            new Vector2Int(0, 1), new Vector2Int(0, -1),
            new Vector2Int(1, 0), new Vector2Int(-1, 0)
        };

        foreach (var dir in directions)
        {
            Vector2Int neighbor = current + dir;
            if (neighbor.x >= 0 && neighbor.x < _gridManager.SizeX && neighbor.y >= 0 && neighbor.y < _gridManager.SizeY)
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    private int Heuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current)
    {
        List<Vector2Int> path = new List<Vector2Int> { current };

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
        Vector2Int closestTarget = new Vector2Int(-1, -1);
        int shortestPathCost = int.MaxValue;

        for (int x = 0; x < _gridManager.SizeY; x++)
        {
            Vector2Int target = new Vector2Int(x, topRow);
            
            if (_gridManager.IsCellOccupied(target)) continue;

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
