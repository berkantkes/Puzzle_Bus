using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class InputFieldValueController : MonoBehaviour
{
    public enum ValueType
    {
        GridWidth,
        GridLength,
        Time
    }
    
    [SerializeField] protected TMP_InputField inputField;
    [SerializeField] protected int _minValue;
    [SerializeField] protected int _maxValue;
    [SerializeField] protected int _defaultValue; 
    
    protected LevelEditorManager _levelEditorManager;
    
    public void Initialize(LevelEditorManager levelEditorManager)
    {
        _levelEditorManager = levelEditorManager;

        // Default değeri inputField text alanına yaz
        if (inputField != null)
        {
            _defaultValue = Mathf.Clamp(_defaultValue, _minValue, _maxValue);
            inputField.text = _defaultValue.ToString();
            HandleValueChanged(_defaultValue);
        }
        
        Debug.Log("Controller initialized with default value: " + _defaultValue);
    }
    
    private void Start()
    {
        if (inputField != null)
        {
            inputField.onEndEdit.AddListener(OnInputFieldValueChanged);
        }
    }

    private void OnInputFieldValueChanged(string value)
    {
        if (!float.TryParse(value, out var result)) return;

        result = Mathf.Clamp(result, _minValue, _maxValue);

        HandleValueChanged(result);
    }
    
    // Bu metot abstract yapılır, türetilen sınıflar kendi özel işlevlerini uygular
    protected abstract void HandleValueChanged(float value);
}