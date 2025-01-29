using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;using System.Linq;


public class BusStopManager : MonoBehaviour
{
    private List<BusStop> _busStops = new List<BusStop>();
    private ObjectPoolManager _poolManager; 

    public void Initialize(ObjectPoolManager poolManager)
    {
        _poolManager = poolManager;
        CreateBusStops();
    }

    private void CreateBusStops()
    {
        int busStopCount = 5;
        float startX = -(float)busStopCount / 2 + .5f;
        float startZ = 1;
        float gap = 1f;

        for (int i = 0; i < 5; i++)
        {
            Vector3 position = new Vector3(startX + (i * gap), 0, startZ);
            
            BusStop busStop = _poolManager.GetFromPool<BusStop>(ObjectType.BusStop, position, Quaternion.identity);
            busStop.Initialize(position);
            busStop.transform.SetParent(transform);
            _busStops.Add(busStop);
        }
    }

    public BusStop GetAvailableBusStop()
    {
        foreach (BusStop busStop in _busStops)
        {
            if (!busStop.IsOccupied)
            {
                return busStop;
            }
        }

        return null;
    }

    public async void CheckLastAvailableBusStop(Human human)
    {
        bool isLastBusStop = _busStops.Count(b => b.IsOccupied) == 4;

        if (isLastBusStop)
        {
            EventManager.Execute(GameEvents.OnLevelFail);
            // await UniTask.WaitUntil(() => human.IsMoving == 0);
            // EventManager.Execute(GameEvents.OnLevelFailAndNotMove);
            
        }
    }

    public List<BusStop> GetBusStops()
    {
        return _busStops;
    }
    
    public void ClearBusStops()
    {
        foreach (BusStop busStop in _busStops)
        {
            if (busStop != null)
            {
                _poolManager.ReturnToPool(ObjectType.BusStop, busStop.gameObject);
            }
        }

        _busStops.Clear();
    }
}
