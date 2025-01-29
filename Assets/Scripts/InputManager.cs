using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    
    private HumanManager _humanManager;
    private GridManager _gridManager;
    private GameManager _gameManager;
    
    public void Initialize(GameManager gameManager, HumanManager humanManager)
    {
        _humanManager = humanManager;
        _gameManager = gameManager;
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && _gameManager.CurrentStatus == GameManager.GameStatus.Gameplay)
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 clickedWorldPosition = hit.point;

                HandleClick(clickedWorldPosition);
            }
        }
    }
    
    private void HandleClick(Vector3 worldPosition)
    {
        _humanManager.MoveHuman(worldPosition);
    }
    
}
