using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class BusController : MonoBehaviour
{
    private ObjectPoolManager _objectPoolManager;
    private List<Bus> _busList = new List<Bus>();
    private List<Bus> _busListCopy = new List<Bus>();
    private int offset = -2;
    
    public void Initialize (LevelData levelData, ObjectPoolManager poolManager)
    {
        CreateBus(levelData, poolManager);
    }

    private void CreateBus(LevelData levelData, ObjectPoolManager poolManager)
    {
        _objectPoolManager = poolManager;
        for (int i = 0; i < levelData.buses.Count; i++)
        {
            Bus bus = _objectPoolManager.GetFromPool<Bus>(ObjectType.Bus, new Vector3(-i * 7.4f, 0, + 3),
                Quaternion.identity);
            bus.Initialize(this, levelData.buses[i]);
            _busList.Add(bus); 
            _busListCopy.Add(bus);
        }
    }

    public Bus GetBus()
    {
        return _busList.First();
    }
    
    public async void MoveBusesAndRemoveFirst(HashSet<Human> humans)
    {
        if (_busList == null || _busList.Count == 0)
        {
            Debug.LogWarning("Bus listesi boş!");
            return;
        }

        await UniTask.WaitUntil(() => humans.All(h => h.IsMoving == 0));
        Sequence busSequence = DOTween.Sequence();

        Bus firstBus = _busList[0];
        busSequence.Join(firstBus.transform.DOMoveX(firstBus.transform.position.x + 20, .2f)
            .SetEase(Ease.Linear));

        _busList.RemoveAt(0);
        for (int i = 0; i < _busList.Count; i++)
        {
            Bus bus = _busList[i];
            busSequence.Join(bus.transform.DOMoveX(bus.transform.position.x + 7.4f, .2f)
                .OnComplete(()=> EventManager.Execute(GameEvents.OnNewBusCome))
                .SetEase(Ease.Linear));
        }
        // for (int i = 0; i < _busList.Count; i++)
        // {
        //     Bus bus = _busList[i];
        //     busSequence.Join(bus.transform.DOMoveX(bus.transform.position.x + 7.4f, .2f)
        //         .OnComplete(()=>
        //         {
        //             _busList.RemoveAt(0);
        //             EventManager.Execute(GameEvents.OnNewBusCome);
        //         })
        //         .SetEase(Ease.Linear));
        // }

        busSequence.OnComplete(() =>
        {
            if (_busList.Count == 0)
            {
                EventManager.Execute(GameEvents.OnLevelSuccessful);
                Debug.Log(GameEvents.OnLevelSuccessful);
            }
            
        });

        busSequence.Play();
    }
    public void ClearBuses()
    {
        foreach (Bus bus in _busListCopy)
        {
            if (bus != null)
            {
                bus.ClearBus();
                _objectPoolManager.ReturnToPool(ObjectType.Bus, bus.gameObject);
            }
        }

        _busList.Clear();
    }
}
