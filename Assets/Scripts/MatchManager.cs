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
            human.MoveToPosition(busStop.position);
            //StartCoroutine(human.MoveToPosition(busStop.position));
            busStop.isOccupied = true;
        }

        //TODO FAIL LEVEL
    }
}
