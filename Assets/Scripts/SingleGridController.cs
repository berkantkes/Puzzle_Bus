using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleGridController : MonoBehaviour
{
    private bool _isItOcuppied = false;

    public bool IsItOcuppied => _isItOcuppied;

    public void SetIsItOcuppied(bool status)
    {
        _isItOcuppied = status;
    }
}
