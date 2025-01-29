using UnityEngine;

public class GridManager : MonoBehaviour
{
    private ObjectPoolManager _poolManager;
    private SingleGridController[,] _gridCells;
    private int _sizeX;
    private int _sizeY;
    private Vector3 _origin;

    public int SizeX => _sizeX;
    public int SizeY => _sizeY;

    private const float CellOffset = 0.5f;
    private float _gridCenterOffsetX;     
    private float _gridBottomOffsetZ;      

    public void Initialize(LevelData levelData, ObjectPoolManager poolManager)
    {
        _poolManager = poolManager;
        _sizeX = levelData.gridSizeX;
        _sizeY = levelData.gridSizeY;
        float boardCenter = _sizeX / 2f;

        _gridCenterOffsetX = -boardCenter + CellOffset; 
        _gridBottomOffsetZ = -_sizeY + CellOffset;        

        _origin = new Vector3(_gridCenterOffsetX, 0, _gridBottomOffsetZ);
        _gridCells = new SingleGridController[_sizeX, _sizeY];

        CreateGrid();
    }

    private void CreateGrid()
    {
        for (int x = 0; x < _sizeX; x++)
        {
            for (int y = 0; y < _sizeY; y++)
            {
                Vector3 worldPosition = GetWorldPosition(new Vector2Int(x, y));
                SingleGridController gridCell = _poolManager.GetFromPool<SingleGridController>(
                    ObjectType.GridCell, worldPosition, Quaternion.identity);
                
                gridCell.transform.SetParent(transform);
                _gridCells[x, y] = gridCell;
            }
        }
    }

    public bool IsCellOccupied(Vector2Int position)
    {
        return IsValidGridPosition(position) && _gridCells[position.x, position.y].IsItOcuppied;
    }

    public void SetCellOccupied(Vector2Int position, bool occupied)
    {
        if (IsValidGridPosition(position))
        {
            _gridCells[position.x, position.y].SetIsItOcuppied(occupied);
        }
    }

    public Vector3 GetWorldPosition(Vector2Int gridPosition)
    {
        return new Vector3(gridPosition.x + _origin.x, 0, gridPosition.y + _origin.z);
    }

    public Vector2Int GetGridPosition(Vector3 worldPosition)
    {
        float x = worldPosition.x - _origin.x;
        float y = worldPosition.z - _origin.z;
        return new Vector2Int(Mathf.RoundToInt(x), Mathf.RoundToInt(y));
    }

    public void ClearGrid()
    {
        if (_gridCells == null) return;

        for (int x = 0; x < _sizeX; x++)
        {
            for (int y = 0; y < _sizeY; y++)
            {
                if (_gridCells[x, y] != null)
                {
                    _poolManager.ReturnToPool(ObjectType.GridCell, _gridCells[x, y].gameObject);
                    _gridCells[x, y] = null;
                }
            }
        }

        _sizeX = 0;
        _sizeY = 0;
        _gridCells = null;
    }

    private bool IsValidGridPosition(Vector2Int position)
    {
        return position.x >= 0 && position.x < _sizeX && position.y >= 0 && position.y < _sizeY;
    }
}
