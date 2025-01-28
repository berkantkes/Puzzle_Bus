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
    [Header("Grid ")]
    public int gridSizeX = 4;
    public int gridSizeY = 4;

    [Header("Human Positions")]
    public List<HumanData> HumanDatas = new List<HumanData>(); 
    
    [Header("Bus")]
    public List<ColorType> buses = new List<ColorType>();
    
    [Header("Time")]
    public float time = 1;
}

[System.Serializable]
public class HumanData
{
    public Vector2Int HumanStartPosition;
    public ColorType HumanColorType;
}