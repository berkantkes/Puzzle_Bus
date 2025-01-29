using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditorLevelManager : MonoBehaviour
{
    [SerializeField] private GameObject levelPrefab;
    [SerializeField] private Transform levelParent;
    [SerializeField] private ScrollRect scrollRect;
        
    private const string PathOfData = "Datas/Levels";
    private List<LevelData> _datas = new List<LevelData>();
    private int _totalLevelCount;
    private LevelEditorManager _levelEditorManager;

    public void Initialize(LevelEditorManager levelEditorManager)
    {
        _levelEditorManager = levelEditorManager;
        CreateLevels();
    }
    
    // public void CreateLevels()
    // {
    //     for(var i = 0; i < levelParent.childCount; i++)
    //     {
    //         Destroy(levelParent.GetChild(i).gameObject);
    //     }
    //         
    //     _datas = GetDatas();
    //     _totalLevelCount = GetTotalLevelCount();
    //     for (var i = 0; i < _totalLevelCount; i++)
    //     {
    //         var level = Instantiate(levelPrefab, levelParent);
    //         var i1 = i;
    //         level.GetComponent<Button>().onClick.AddListener(() => _levelEditorManager.LoadLevel(_datas[i1].levelNumber));
    //         level.GetComponentsInChildren<TextMeshProUGUI>().First().text = $"Level {_datas[i].levelNumber}";
    //     }
    //     scrollRect.verticalNormalizedPosition = 0;
    // }
    
    public void CreateLevels()
    {
        // Mevcut seviyeleri temizle
        for (var i = 0; i < levelParent.childCount; i++)
        {
            Destroy(levelParent.GetChild(i).gameObject);
        }
            
        // Verileri al
        _datas = GetDatas();
        if (_datas.Count == 0)
        {
            Debug.LogWarning("Hiç level bulunamadı. Lütfen Resources klasörünü kontrol edin.");
            return;
        }

        _totalLevelCount = _datas.Count;
        for (var i = 0; i < _totalLevelCount; i++)
        {
            // Level butonlarını oluştur
            var level = Instantiate(levelPrefab, levelParent);
            var i1 = i;
            level.GetComponent<Button>().onClick.AddListener(() => _levelEditorManager.LoadLevel(_datas[i1].levelNumber));
            level.GetComponentsInChildren<TextMeshProUGUI>().First().text = $"Level {_datas[i].levelNumber}";
        }

        // Scroll konumunu sıfırla
        scrollRect.verticalNormalizedPosition = 1; // En üstte başlasın
    }

            
    private int GetTotalLevelCount()
    {
        return _datas.Count;
    }
    
    public int DetectMinimumMissingLevelID()
    {
        if(_datas.Count == 0) return 1;
        var maxLevel = _datas.Max(data => data.levelNumber);
        if(maxLevel == 0) return 1;
        var allLevels = Enumerable.Range(1, maxLevel).ToList();
        var existingLevels = _datas.Select(data => data.levelNumber).ToList();

        var missingLevels = allLevels.Except(existingLevels).ToList();

        return missingLevels.Any() ? missingLevels.Min() : maxLevel + 1;
    }
    
    public LevelData GetLevelData(int id)
    {
        return _datas.FirstOrDefault(data => data.levelNumber == id);
    }

    // public List<LevelData> GetDatas()
    // {
    //     return _datas = new List<LevelData>(Resources.LoadAll<Levels>(PathOfData)
    //         .Select(item => item.GetData())
    //         .OrderBy(data => data.levelNumber).ToList());
    // }
    
    public List<LevelData> GetDatas()
    {
        var levels = Resources.LoadAll<Levels>(PathOfData);

        if (levels == null || levels.Length == 0)
        {
            Debug.LogError($"Resources klasöründen Datas/Levels altındaki Levels varlıkları yüklenemedi!");
            return new List<LevelData>();
        }

        return new List<LevelData>(levels.Select(item => item.GetData())
            .OrderBy(data => data.levelNumber).ToList());
    }

    
}
