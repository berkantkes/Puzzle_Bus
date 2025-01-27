using UnityEngine;

public class BusStop : MonoBehaviour
{
    public Vector3 position;
    public bool isOccupied;

    public void Initialize(Vector3 position)
    {
        this.position = position;
        this.isOccupied = false;
    }
}