using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassangerEditor : MonoBehaviour
{
    [SerializeField] private Renderer _renderer;

    private Material[] _originalMaterials;
    private ColorType _colorType;

    public ColorType ColorType => _colorType;
    
    public void Initialize(ColorType colorType)
    {
        _originalMaterials = new Material[1];
        _originalMaterials[0] = ColorMaterialSelector.GetColorMaterial(colorType); 

        _renderer.materials = _originalMaterials; 
        _colorType = colorType;           
    }
}
