using UnityEngine;

public class TimeController : InputFieldValueController
{
    protected override void HandleValueChanged(float value)
    {
        _levelEditorManager.SetGridTime(value);
    }
}

