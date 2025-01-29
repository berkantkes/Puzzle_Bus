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
    
    private void OnEnable()
    {
        EventManager.Subscribe(GameEvents.OnNewBusCome, CheckMatchNewBus);
    }

    private void OnDisable()
    {
        EventManager.Unsubscribe(GameEvents.OnNewBusCome, CheckMatchNewBus);
    }
    
    public void MoveToBusStop(Human human, GridManager gridManager)
    {
        Bus bus = _busController.GetBus();
        BusStop busStop = _busStopManager.GetAvailableBusStop();
        
        if (bus.ColorType == human.ColorType)
        {
            human.MoveToBusPosition(bus.GetBusOpenPosition(), bus);
            //StartCoroutine(human.MoveToPosition(bus.GetBusOpenPosition()));
            bus.AddNewHuman(human);
        }
        else
        {
            human.MoveToBusStopPosition(busStop.GetPosition());
            _busStopManager.CheckLastAvailableBusStop(human);
            busStop.AttachHuman(human);
            //StartCoroutine(human.MoveToPosition(busStop.position));
        }

        //TODO FAIL LEVEL
    }
    
    private void CheckMatchNewBus()
    {
        Debug.Log("CheckMatchNewBus");
        foreach (BusStop busStop in _busStopManager.GetBusStops())
        {
            Debug.Log("CheckMatchNewBus 1");
            if (busStop != null)
            {
                Bus bus = _busController.GetBus();
                Human human = busStop.GetAttachHuman();
                Debug.Log("CheckMatchNewBus 2");
                Debug.Log("CheckMatchNewBus 2 human.ColorType" + human.ColorType);
                Debug.Log("CheckMatchNewBus 2 bus.ColorType" + bus.ColorType);
                if (bus.ColorType == human.ColorType)
                {
                    Debug.Log("CheckMatchNewBus 3");
                    human.MoveToBusPosition(bus.GetBusOpenPosition(), bus);
                    //StartCoroutine(human.MoveToPosition(bus.GetBusOpenPosition()));
                    bus.AddNewHuman(human);
                    busStop.SetIsOccupied(false);
                }
            }
        }
    }
    
}
