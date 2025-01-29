using UnityEngine;

public class BusStop : MonoBehaviour
{
    private Vector3 _position;
    private Human _attachedHuman;

    public Vector3 Position => _position;
    public bool IsOccupied => _attachedHuman != null;
    public Human AttachedHuman => _attachedHuman;

    public void Initialize(Vector3 position)
    {
        _position = position;
    }

    public void AttachHuman(Human human)
    {
        if (_attachedHuman == null)
        {
            _attachedHuman = human;
        }
    }

    public void DetachHuman()
    {
        _attachedHuman = null;
    }
}