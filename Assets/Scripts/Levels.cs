using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Levels", menuName = "Game/Levels")]
public class Levels : ScriptableObject
{
    [Header("Tüm Level Bilgileri")]
    public List<LevelData> levels = new List<LevelData>();
}

[System.Serializable]
public class LevelData
{
    [Header("Grid Boyutları")]
    public int gridSizeX = 4;
    public int gridSizeY = 4;

    [Header("Human Pozisyonları")]
    public List<Vector2Int> humanStartPositions = new List<Vector2Int>(); 
    
    [Header("Otobüs Verileri")]
    public List<ColorType> buses = new List<ColorType>();
}

