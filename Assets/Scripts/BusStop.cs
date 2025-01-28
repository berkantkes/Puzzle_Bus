using UnityEngine;

public class BusStop : MonoBehaviour
{
    private Vector3 _position;
    private bool _isOccupied;
    private Human _attachedHuman;

    public void Initialize(Vector3 position)
    {
        _position = position;
        _isOccupied = false;
    }

    public Vector3 GetPosition()
    {
        return _position;
    }
    public bool GetIsOccupied()
    {
        return _isOccupied;
    }
    public void SetIsOccupied(bool status)
    {
        _isOccupied = status;
    }
    public Human GetAttachHuman()
    {
        return _attachedHuman;
    }
    public void AttachHuman(Human human)
    {
        _attachedHuman = human;
        _isOccupied = true;
    }
}