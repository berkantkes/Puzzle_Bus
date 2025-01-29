using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleGridController : MonoBehaviour
{
    private bool _isItOcuppied = false;
    private GridTypes _gridType;

    public bool IsItOcuppied => _isItOcuppied;
    public GridTypes GridType => _gridType;

    public void SetIsItOcuppied(bool status)
    {
        _isItOcuppied = status;
    }
}
