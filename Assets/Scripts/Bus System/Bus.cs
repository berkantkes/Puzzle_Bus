using System.Collections.Generic;
using UnityEngine;

public class Bus : MonoBehaviour
{
    [SerializeField] private Transform _busOpenPosition;
    [SerializeField] private Renderer _renderer;

    private ColorType _colorType;
    private BusController _busController;
    private HashSet<Human> _humanList = new HashSet<Human>();

    private const int MaxCapacity = 3;

    public ColorType ColorType => _colorType;
    public Vector3 BusOpenPosition => _busOpenPosition.position;
    public bool IsFull => _humanList.Count >= MaxCapacity;

    public void Initialize(BusController busController, ColorType colorType)
    {
        _busController = busController;
        _colorType = colorType;
        SetBusColor();
    }

    private void SetBusColor()
    {
        _renderer.material = ColorMaterialSelector.GetColorMaterial(_colorType);
    }

    public void AddPassenger(Human human)
    {
        if (!_humanList.Contains(human))
        {
            _humanList.Add(human);
            if (IsFull)
            {
                _busController.MoveBusesAndRemoveFirst(_humanList);
            }
        }
    }

    public void ClearBus()
    {
        _humanList.Clear();
    }
}