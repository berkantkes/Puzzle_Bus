using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    private BusController _busController;
    private BusStopManager _busStopManager;
    
    public void Initialize(BusController busController, BusStopManager busStopManager)
    {
        _busController = busController;
        _busStopManager = busStopManager;
    }
    
    public void MoveToBusStop(Human human, GridManager gridManager)
    {
        Debug.Log("MoveToBusStop 1 ");
        Bus bus = _busController.GetBus();
        BusStop busStop = _busStopManager.GetAvailableBusStop();
        
        if (bus.ColorType == human.ColorType)
        {
            human.MoveToPosition(bus.GetBusOpenPosition());
            //StartCoroutine(human.MoveToPosition(bus.GetBusOpenPosition()));
            bus.AddNewHuman();
        }
        else
        {
            Debug.Log("MoveToBusStop 2 ");
            human.MoveToPosition(busStop.position);
            _busStopManager.CheckLastAvailableBusStop(human);
            busStop.isOccupied = true;
            //StartCoroutine(human.MoveToPosition(busStop.position));
        }

        //TODO FAIL LEVEL
    }
}
