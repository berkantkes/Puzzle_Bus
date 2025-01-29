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
    // [SerializeField] private PassangerEditor _blackPassanger; 
    // [SerializeField] private PassangerEditor _redPassanger; 
    // [SerializeField] private PassangerEditor _yellowPassanger; 
    // [SerializeField] private PassangerEditor _pinkPassanger; 
    // [SerializeField] private PassangerEditor _purplePassanger; 
    //[SerializeField] private Button _editorLevelManager;
    
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
                    Debug.Log("berkant");
                    PassangerEditor passangerEditor = SelectPassangerDropdown();
            
                    if (passangerEditor != null)
                    {
                        Vector2Int currentPosition = _gridManager.GetGridPosition(hit.point);
                        ColorType colorType = passangerEditor.ColorType;

                        // Önce listeye ekleyelim (kaydetme işlemi için)
                        _humanDatas.Add(new HumanData(currentPosition, colorType));

                        // **Sahnede anında göstermeyi sağlayan kod**
                        AddPassengerToScene(currentPosition, colorType);

                        // Level kaydını yap
                        //SaveLevel();
                    }
                }
            }
        }
    }
    private void AddPassengerToScene(Vector2Int gridPosition, ColorType colorType)
    {
        // Grid'deki dünya koordinatını al
        Vector3 worldPosition = _gridManager.GetWorldPosition(gridPosition);

        // Yeni bir PassengerEditor nesnesi oluştur
        PassangerEditor newPassenger = Instantiate(_passanger, worldPosition, Quaternion.identity, _gridManager.transform);
    
        // Passenger'a doğru rengi ata
        newPassenger.Initialize(colorType);

        Debug.Log($"Yeni yolcu oluşturuldu: {colorType} rengiyle {gridPosition} konumunda.");
    }

    private PassangerEditor SelectPassangerDropdown()
    {
        PassangerEditor passangerEditor = _passanger;
        
        switch (_selectIndex)
        {
            case 0:
                //passangerEditor = _whitePassanger;
                passangerEditor.Initialize(ColorType.White);
                break;
            case 1:
               // passangerEditor = _blackPassanger;
                passangerEditor.Initialize(ColorType.Black);
                break;
            case 2:
               // passangerEditor = _redPassanger;
                passangerEditor.Initialize(ColorType.Red);
                break;
            case 3:
               // passangerEditor = _yellowPassanger;
                passangerEditor.Initialize(ColorType.Yellow);
                break;
            case 4:
               // passangerEditor = _pinkPassanger;
                passangerEditor.Initialize(ColorType.Pink);
                break;
            case 5:
              //  passangerEditor = _purplePassanger;
                passangerEditor.Initialize(ColorType.Purple);
                break;
            case 6:
                passangerEditor = null;
                break;
            default:
                Debug.Log("Other option selected");
                break;
        }

        return passangerEditor;
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
        // ClearGrid();
        // ClearBuses();
        Debug.Log("CreateGrid");
        _gridManager.ClearGrid();
        UpdateLevelData();
        _gridManager.Initialize(_levelData, _objectPoolManager);
        //SaveLevel();
    }

    public void LoadLevel(int number)
    {
        Debug.Log("LoadLevel : " + number);
        var data = _editorLevelManager.GetDatas().FirstOrDefault(data => data.levelNumber == number);
        if (data is null)
        {
            //CreateNewLevel();
            return;
        }

        ClearPassengers();
        levelNumber = data.levelNumber;
        _gridWidth = data.gridSizeX;
        _gridHeight = data.gridSizeY;
        _time = data.time;
        _humanDatas = data.HumanDatas;
        _buses = data.buses;
        // busesEditor = new List<BusArea>(data.buses);
        // cellsEditor = new List<CellArea>(data.cells);
        CreateGrid();
        PlacePassengersOnGrid(_humanDatas);
        //UpdateLevelUI(levelIDEditor, $"Level {levelIDEditor} loaded!");
                
        //_currentLevelData = data;
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
        Debug.Log("Tüm yolcular temizlendi.");
    }
    
    
    private void PlacePassengersOnGrid(List<HumanData> humanDatas)
    {
        foreach (var humanData in humanDatas)
        {
            // HumanData içindeki pozisyon bilgisine göre grid konumunu al
            Vector2Int gridPosition = humanData.HumanStartPosition;
            ColorType colorType = humanData.HumanColorType;

            // Grid üzerindeki dünya koordinatını al
            Vector3 worldPosition = _gridManager.GetWorldPosition(gridPosition);

            // Passenger'ı oluştur
            PassangerEditor newPassenger = Instantiate(_passanger, worldPosition, Quaternion.identity, _gridManager.transform);
            newPassenger.Initialize(colorType);
        
            Debug.Log($"Passenger {colorType} rengiyle {gridPosition} konumuna yerleştirildi.");
        }
    }

    public void CreateNewLevel()
    {
        int minLevelNumber = _editorLevelManager.DetectMinimumMissingLevelID();
            
        _gridWidth = 3;
        _gridHeight = 3;
        _time = 30;
        _humanDatas.Clear();
        // cellsEditor.Clear();
        // busesEditor.Clear();
        // cellsEditor = new List<CellArea>(_onGridCreator.CreateGrid(gridWidthEditor, gridLengthEditor, new List<CellArea>(9)));
            
        var newLevelData = new LevelData
        {
            levelNumber = minLevelNumber,
            gridSizeX = _gridWidth,
            gridSizeY = _gridHeight,
            time = _time,
            HumanDatas = _humanDatas,
        };
        
        CreateAndSaveLevelAsset(newLevelData);
        CreateGrid();
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
        // Mevcut veriyi oluştur
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

        // Mevcut varlığı güncelle
        UpdateExistingLevel(updatedLevelData);
    }
    public void UpdateExistingLevel(LevelData updatedLevelData)
    {
        var existingLevelAsset = GetAllLevels()
            .FirstOrDefault(level => level.Data.levelNumber == updatedLevelData.levelNumber);

        if (existingLevelAsset == null)
        {
            Debug.LogError($"Level {updatedLevelData.levelNumber} bulunamadı!");
            return;
        }

        // Mevcut veriyi güncelle
        existingLevelAsset.Data.gridSizeX = updatedLevelData.gridSizeX;
        existingLevelAsset.Data.gridSizeY = updatedLevelData.gridSizeY;
        existingLevelAsset.Data.time = updatedLevelData.time;
        existingLevelAsset.Data.HumanDatas = new List<HumanData>(updatedLevelData.HumanDatas);
        existingLevelAsset.Data.buses = new List<ColorType>(updatedLevelData.buses);

        // ScriptableObject'i kirli olarak işaretle
        EditorUtility.SetDirty(existingLevelAsset);

        // Varlık veritabanındaki değişiklikleri kaydet
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Level {updatedLevelData.levelNumber} başarıyla güncellendi!");
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
