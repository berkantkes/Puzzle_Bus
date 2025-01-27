using UnityEngine;

[System.Serializable]
public class PoolItem
{
    public ObjectType type; 
    public GameObject prefab;
    public int amount;      
}

[CreateAssetMenu(fileName = "NewPoolData", menuName = "Pooling/Pool Data")]
public class PoolData : ScriptableObject
{
    public PoolItem[] items;
}