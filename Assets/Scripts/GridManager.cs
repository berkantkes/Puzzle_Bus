using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    private ObjectPoolManager _poolManager;
    private SingleGridController[,] gridCells;
    private int _sizeX;
    private int _sizeY;
    private Vector3 _origin; 
    
    public int SizeX => _sizeX;
    public int SizeY => _sizeY;

    public void Initialize(LevelData levelData, ObjectPoolManager poolManager)
    {
        _poolManager = poolManager;
        _sizeX = levelData.gridSizeX;
        _sizeY = levelData.gridSizeY;

        float offsetX = (-(_sizeX ) / 2f) + .5f;
        float offsetZ = -_sizeY + .5f;
        _origin = new Vector3(offsetX, 0, offsetZ);
        gridCells = new SingleGridController[_sizeX, _sizeY];
        CreateGridVisual();
    }
    
    private void CreateGridVisual()
    {
        for (int x = 0; x < _sizeX; x++)
        {
            for (int y = 0; y < _sizeY; y++)
            {
                Vector3 worldPosition = GetWorldPosition(new Vector2Int(x, y));
                SingleGridController gridCell = _poolManager.GetFromPool<SingleGridController>(ObjectType.GridCell, worldPosition, Quaternion.identity);
                gridCell.transform.SetParent(transform);
                gridCells[x, y] = gridCell;
            }
        }
    }

    public bool IsCellOccupied(Vector2Int position)
    {
        Debug.Log(position.x + "," + position.y);
        return gridCells[position.x, position.y].IsItOcuppied;
    }

    public void SetCellOccupied(Vector2Int position, bool occupied)
    {
        gridCells[position.x, position.y].SetIsItOcuppied(occupied);
    }

    public Vector3 GetWorldPosition(Vector2Int gridPosition)
    {
        float x = gridPosition.x + _origin.x;
        float z = gridPosition.y + _origin.z;
        return new Vector3(x, 0, z);
    }

    public Vector2Int GetGridPosition(Vector3 worldPosition)
    {
        float x = worldPosition.x - _origin.x;
        float y = worldPosition.z - _origin.z;
        return new Vector2Int(Mathf.RoundToInt(x), Mathf.RoundToInt(y));
    }
    
    public void ClearGrid()
    {
        if (gridCells == null) return;

        for (int x = 0; x < _sizeX; x++)
        {
            for (int y = 0; y < _sizeY; y++)
            {
                if (gridCells[x, y] != null)
                {
                    _poolManager.ReturnToPool(ObjectType.GridCell, gridCells[x, y].gameObject);
                    gridCells[x, y] = null;
                }
            }
        }

        _sizeX = 0;
        _sizeY = 0;
        gridCells = null;
    }
}