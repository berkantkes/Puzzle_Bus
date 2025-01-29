using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class LevelEditorManager : MonoBehaviour
{
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private ObjectPoolManager _objectPoolManager;
    [SerializeField] private List<InputFieldValueController> _inputFieldList;
    [SerializeField] private LevelData _levelData;
    [SerializeField] private EditorLevelManager _editorLevelManager;
    [SerializeField] private Button _createNewLevelButton;
    [SerializeField] private Button _saveLevelButton;
    [SerializeField] private Camera _camera;
    [SerializeField] private TMP_Dropdown _dropdown;
    [SerializeField] private PassangerEditor _passanger; 
    [SerializeField] private ColorMaterialSelector _colorMaterialSelector;
    [SerializeField] private TMP_Text _levelText;
    
    private const string LevelFileName = "Level_";
    private int _gridWidth = 3;
    private int _gridHeight = 3;
    private float _time = 3;
    public int levelNumber = 1;
    private int _selectIndex = 0;
    private List<HumanData> _humanDatas = new List<HumanData>();
    private List<ColorType> _buses = new List<ColorType>();

    private void Awake()
    {
        foreach (var inputfield in _inputFieldList)
        {
            inputfield.Initialize(this);
        }

        _editorLevelManager.Initialize(this);
        _colorMaterialSelector.Initialize();
        LoadLevel(1);
    }
    

    private void Update()
    {
        var ray = _camera.ScreenPointToRay(Input.mousePosition);
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out var hit))
            {
                if (hit.transform.TryGetComponent(out SingleGridController tileEditor))
                {
                    PassangerEditor passangerEditor = SelectPassangerDropdown();
            
                    if (passangerEditor != null)
                    {
                        Vector2Int currentPosition = _gridManager.GetGridPosition(hit.point);
                        ColorType colorType = passangerEditor.ColorType;

                        _humanDatas.Add(new HumanData(currentPosition, colorType));

                        AddPassengerToScene(currentPosition, colorType);
                    }
                }
            }
        }
    }
    private void AddPassengerToScene(Vector2Int gridPosition, ColorType colorType)
    {
        Vector3 worldPosition = _gridManager.GetWorldPosition(gridPosition);

        PassangerEditor newPassenger = Instantiate(_passanger, worldPosition, Quaternion.identity, _gridManager.transform);
    
        newPassenger.Initialize(colorType);

    }
    private PassangerEditor SelectPassangerDropdown()
    {
        var colorMapping = new Dictionary<int, ColorType>
        {
            { 0, ColorType.White },
            { 1, ColorType.Black },
            { 2, ColorType.Red },
            { 3, ColorType.Yellow },
            { 4, ColorType.Pink },
            { 5, ColorType.Purple }
        };

        if (_selectIndex == 6)
            return null;

        if (colorMapping.TryGetValue(_selectIndex, out var selectedColor))
        {
            _passanger.Initialize(selectedColor);
        }
        else
        {
            Debug.Log("Other option selected");
        }

        return _passanger;
    }


    private void OnDropdownValueChanged(int selectedIndex)
    {
        _selectIndex = selectedIndex;
        
    }

    private void OnEnable()
    {
        _createNewLevelButton.onClick.AddListener(CreateNewLevel);
        _saveLevelButton.onClick.AddListener(SaveLevel);
        _dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    private void OnDisable()
    {
        _createNewLevelButton.onClick.RemoveListener(CreateNewLevel);
        _saveLevelButton.onClick.RemoveListener(SaveLevel);
        _dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    public void CreateGrid()
    {
        _gridManager.ClearGrid();
        UpdateLevelData();
        _gridManager.Initialize(_levelData, _objectPoolManager);
    }

    public void LoadLevel(int number)
    {
        var data = _editorLevelManager.GetLevels().FirstOrDefault(data => data.levelNumber == number);
        if (data is null)
        {
            return;
        }

        ClearPassengers();
        levelNumber = data.levelNumber;
        _gridWidth = data.gridSizeX;
        _gridHeight = data.gridSizeY;
        _time = data.time;
        _humanDatas = data.HumanDatas;
        _buses = data.buses;
        CreateGrid();
        PlacePassengersOnGrid(_humanDatas);
        _levelText.SetText("LEVEL " + levelNumber);
    }
    
    private void ClearPassengers()
    {
        foreach (Transform child in _gridManager.transform)
        {
            if (child.TryGetComponent<PassangerEditor>(out var passenger))
            {
                Destroy(passenger.gameObject); // Mevcut PassangerEditor nesnelerini sil
            }
        }
    }
    
    
    private void PlacePassengersOnGrid(List<HumanData> humanDatas)
    {
        foreach (var humanData in humanDatas)
        {
            Vector2Int gridPosition = humanData.HumanStartPosition;
            ColorType colorType = humanData.HumanColorType;

            Vector3 worldPosition = _gridManager.GetWorldPosition(gridPosition);

            PassangerEditor newPassenger = Instantiate(_passanger, worldPosition, Quaternion.identity, _gridManager.transform);
            newPassenger.Initialize(colorType);
        
        }
    }

    public void CreateNewLevel()
    {
        int minLevelNumber = _editorLevelManager.DetectMinimumMissingLevelID();
            
        _gridWidth = 3;
        _gridHeight = 3;
        _time = 30;
        _humanDatas.Clear();
        var newLevelData = new LevelData
        {
            levelNumber = minLevelNumber,
            gridSizeX = _gridWidth,
            gridSizeY = _gridHeight,
            time = _time,
            HumanDatas = _humanDatas,
        };
        
        ClearPassengers();
        CreateAndSaveLevelAsset(newLevelData);
        CreateGrid();
        LoadLevel(minLevelNumber);
    }
    
    private void CreateAndSaveLevelAsset(LevelData levelData)
    {
        var levelAsset = ScriptableObject.CreateInstance<Levels>();
        levelAsset.Data = levelData;
        AssetDatabase.CreateAsset(levelAsset, $"Assets/Resources/Datas/Levels/{LevelFileName}{levelData.levelNumber}.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        _editorLevelManager.CreateLevels();
    }
    
    public void SaveLevel()
    {
        Debug.Log(_levelData.buses.Count);
        var updatedLevelData = new LevelData
        {
            levelNumber = levelNumber,
            gridSizeX = _gridWidth,
            gridSizeY = _gridHeight,
            time = _time,
            HumanDatas = new List<HumanData>(_humanDatas),
            buses = new List<ColorType>(_buses)
        };
        Debug.Log(updatedLevelData.buses.Count);

        UpdateExistingLevel(updatedLevelData);
    }
    public void UpdateExistingLevel(LevelData updatedLevelData)
    {
        var existingLevelAsset = GetAllLevels()
            .FirstOrDefault(level => level.Data.levelNumber == updatedLevelData.levelNumber);

        if (existingLevelAsset == null)
        {
            return;
        }

        existingLevelAsset.Data.gridSizeX = updatedLevelData.gridSizeX;
        existingLevelAsset.Data.gridSizeY = updatedLevelData.gridSizeY;
        existingLevelAsset.Data.time = updatedLevelData.time;
        existingLevelAsset.Data.HumanDatas = new List<HumanData>(updatedLevelData.HumanDatas);
        existingLevelAsset.Data.buses = new List<ColorType>(updatedLevelData.buses);

        EditorUtility.SetDirty(existingLevelAsset);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

    }

    private Levels[] GetAllLevels()
    {
        return Resources.LoadAll<Levels>("Datas/Levels");
    }

    
    private void UpdateLevelData()
    {
        _levelData.gridSizeX = _gridWidth;
        _levelData.gridSizeY = _gridHeight;
        _levelData.time = _time;
    }

    public void SetGridWidth(int width)
    {
        _gridWidth = width;
    }
    public void SetGridHeight(int height)
    {
        _gridHeight = height;
    }
    public void SetGridTime(float time)
    {
        _time = time;
    }
}
