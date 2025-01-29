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
        EventManager.Subscribe(GameEvents.OnNewBusCome, CheckMatchForNewBus);
    }

    private void OnDisable()
    {
        EventManager.Unsubscribe(GameEvents.OnNewBusCome, CheckMatchForNewBus);
    }

    public void MoveToBusStop(Human human, GridManager gridManager)
    {
        Bus bus = _busController.GetBus();
        BusStop busStop = _busStopManager.GetAvailableBusStop();

        if (bus == null || busStop == null)
        {
            Debug.LogWarning("Bus or BusStop not found! Human cannot be assigned.");
            return;
        }

        if (bus.ColorType == human.ColorType)
        {
            AssignHumanToBus(human, bus);
        }
        else
        {
            AssignHumanToBusStop(human, busStop);
        }
    }

    private void AssignHumanToBus(Human human, Bus bus)
    {
        human.MoveToBusPosition(bus.BusOpenPosition, bus);
        bus.AddPassenger(human);
    }

    private void AssignHumanToBusStop(Human human, BusStop busStop)
    {
        human.MoveToBusStopPosition(busStop.Position);
        _busStopManager.CheckLastAvailableBusStop(human);
        busStop.AttachHuman(human);
    }

    private void CheckMatchForNewBus()
    {
        foreach (BusStop busStop in _busStopManager.GetBusStops())
        {
            if (busStop != null && busStop.AttachedHuman != null)
            {
                Bus bus = _busController.GetBus();
                Human human = busStop.AttachedHuman;

                if (bus == null)
                {
                    Debug.LogWarning("No bus available for matching.");
                    return;
                }

                if (bus.ColorType == human.ColorType)
                {
                    AssignHumanToBus(human, bus);
                    busStop.DetachHuman();
                }
            }
        }
    }
}
