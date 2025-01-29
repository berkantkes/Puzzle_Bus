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
    private LevelEditorManager _levelEditorManager;

    public void Initialize(LevelEditorManager levelEditorManager)
    {
        _levelEditorManager = levelEditorManager;
        CreateLevels();
    }

    public void CreateLevels()
    {
        ClearExistingLevels();
        
        List<LevelData> levelDatas = GetLevels();
        if (!levelDatas.Any()) return;

        PopulateLevels(levelDatas);
        ResetScrollPosition();
    }

    private void ClearExistingLevels()
    {
        foreach (Transform child in levelParent)
        {
            Destroy(child.gameObject);
        }
    }

    private void PopulateLevels(List<LevelData> levelDatas)
    {
        foreach (var levelData in levelDatas)
        {
            var level = Instantiate(levelPrefab, levelParent);
            level.GetComponent<Button>().onClick.AddListener(() => _levelEditorManager.LoadLevel(levelData.levelNumber));
            level.GetComponentsInChildren<TextMeshProUGUI>().First().text = $"Level {levelData.levelNumber}";
        }
    }

    private void ResetScrollPosition()
    {
        scrollRect.verticalNormalizedPosition = 1;
    }

    public List<LevelData> GetLevels()
    {
        var levels = Resources.LoadAll<Levels>(PathOfData);

        if (levels == null || levels.Length == 0)
        {
            return new List<LevelData>();
        }

        return levels.Select(item => item.GetData()).OrderBy(data => data.levelNumber).ToList();
    }

    public int DetectMinimumMissingLevelID()
    {
        List<LevelData> levels = GetLevels();
        if (!levels.Any()) return 1;

        int maxLevel = levels.Max(data => data.levelNumber);
        var allLevels = Enumerable.Range(1, maxLevel).ToList();
        var existingLevels = levels.Select(data => data.levelNumber).ToList();
        var missingLevels = allLevels.Except(existingLevels).ToList();

        return missingLevels.Any() ? missingLevels.Min() : maxLevel + 1;
    }
}
