using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bus : MonoBehaviour
{
    //[SerializeField] private List<Material> _materialList;
    [SerializeField] private Transform _busOpenPosition;
    [SerializeField] private Renderer _renderer;
    private ColorType _colorType;
    private int _humanCount = 0;
    private BusController _busController;
    private HashSet<Human> _humanList = new HashSet<Human>();

    public ColorType ColorType => _colorType;

    public void Initialize(BusController busController, ColorType colorType)
    {
        _busController = busController;
        _colorType = colorType;
        
        SetColorType();
    }
    
    private void SetColorType()
    {
        _renderer.material = ColorMaterialSelector.GetColorMaterial(_colorType);
    }
    
    public Vector3 GetBusOpenPosition()
    {
        return _busOpenPosition.position;
    }

    public void AddNewHuman(Human human)
    {
        _humanCount++;
        _humanList.Add(human);
        if (_humanCount == 3)
        {
            _busController.MoveBusesAndRemoveFirst(_humanList);
        }
    }


    public void ClearBus()
    {
        _humanList.Clear();
    }
}
