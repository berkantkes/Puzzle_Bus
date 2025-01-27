using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusStopManager : MonoBehaviour
{
    private List<BusStop> busStops = new List<BusStop>();
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
            // BusStop busStop = new BusStop(position);
            // busStops.Add(busStop);
            //
            // // Otobüs prefab'ini yerleştir
            // GameObject busStopObject = Instantiate(busStopPrefab, position, Quaternion.identity, busStopParent);
            
            BusStop busStop = _poolManager.GetFromPool<BusStop>(ObjectType.BusStop, position, Quaternion.identity);
            busStop.Initialize(position);
            busStop.transform.SetParent(transform);
            busStops.Add(busStop);
        }
    }

    public BusStop GetAvailableBusStop()
    {
        foreach (BusStop busStop in busStops)
        {
            if (!busStop.isOccupied)
            {
                return busStop;
            }
        }

        return null;
    }
    //TODO CHECK BUSSTOPS isOccupied COUNT
}
