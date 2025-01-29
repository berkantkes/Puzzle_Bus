using UnityEngine;

public class GridHeightController : InputFieldValueController
{
    protected override void HandleValueChanged(float value)
    {
        _levelEditorManager.SetGridHeight((int)value);
        _levelEditorManager.CreateGrid();
        
    }
}

