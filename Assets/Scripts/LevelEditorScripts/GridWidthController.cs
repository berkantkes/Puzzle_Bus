using UnityEngine;

public class GridWidthController : InputFieldValueController
{
    protected override void HandleValueChanged(float value)
    {
        _levelEditorManager.SetGridWidth((int)value);
        _levelEditorManager.CreateGrid();
        
    }
}