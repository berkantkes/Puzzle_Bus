using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bus : MonoBehaviour
{
    //[SerializeField] private List<Material> _materialList;
    [SerializeField] private Transform _busOpenPosition;
    private ColorType _colorType;
    private int _humanCount = 0;
    private BusController _busController;

    public ColorType ColorType => _colorType;

    public void Initialize(BusController busController)
    {
        _busController = busController;
    }
    
    public void SetColorType(ColorType colorType)
    {
        _colorType = colorType;
    }
    
    public Vector3 GetBusOpenPosition()
    {
        return _busOpenPosition.position;
    }

    public void AddNewHuman()
    {
        _humanCount++;
        if (_humanCount == 3)
        {
            _busController.MoveBusesAndRemoveFirst();
        }
    }
}
