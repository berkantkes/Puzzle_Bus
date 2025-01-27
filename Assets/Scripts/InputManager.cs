using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private HumanManager _humanManager;
    private GridManager _gridManager;
    
    public void Initialize(HumanManager humanManager)
    {
        _humanManager = humanManager;
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 clickedWorldPosition = hit.point;

                HandleClick(clickedWorldPosition);
            }
        }
    }
    
    private void HandleClick(Vector3 worldPosition)
    {
        _humanManager.HandleClick(worldPosition);
    }
    
}
