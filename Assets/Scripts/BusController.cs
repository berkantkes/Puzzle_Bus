using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class BusController : MonoBehaviour
{
    private List<Bus> _busList = new List<Bus>();
    private int offset = -2;
    
    public void Initialize (LevelData levelData, ObjectPoolManager poolManager)
    {
        CreateBus(levelData, poolManager);
    }

    private void CreateBus(LevelData levelData, ObjectPoolManager poolManager)
    {
        for (int i = 0; i < levelData.buses.Count; i++)
        {
            Bus bus = poolManager.GetFromPool<Bus>(ObjectType.Bus, new Vector3(-i * 4, 0, + 3),
                Quaternion.identity);
            bus.Initialize(this);
            bus.SetColorType(levelData.buses[i]);
            _busList.Add(bus); 
        }
    }

    public Bus GetBus()
    {
        return _busList.First();
    }
    
    public void MoveBusesAndRemoveFirst()
    {
        if (_busList == null || _busList.Count == 0)
        {
            Debug.LogWarning("Bus listesi boş!");
            return;
        }

        Sequence busSequence = DOTween.Sequence();

        Bus firstBus = _busList[0];
        busSequence.Append(firstBus.transform.DOMoveX(firstBus.transform.position.x + 10, 2f)
            .SetEase(Ease.Linear));

        for (int i = 1; i < _busList.Count; i++)
        {
            Bus bus = _busList[i];
            busSequence.Join(bus.transform.DOMoveX(bus.transform.position.x + 4, 2f).SetEase(Ease.Linear));
        }

        busSequence.OnComplete(() =>
        {
            _busList.RemoveAt(0);

            if (_busList.Count == 0)
            {
                Debug.Log("LEVEL COMPLETED!");
            }
            
        });

        busSequence.Play();
    }
    
}
