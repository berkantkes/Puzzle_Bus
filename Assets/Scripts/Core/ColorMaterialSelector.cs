using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ColorMaterialSelector : MonoBehaviour
{
    [SerializeField] private Material _whiteMaterial;
    [SerializeField] private Material _blackMaterial;
    [SerializeField] private Material _redMaterial;
    [SerializeField] private Material _yellowMaterial;
    [SerializeField] private Material _pinkMaterial;
    [SerializeField] private Material _purpleMaterial;

    private static Dictionary<ColorType, Material> _colorMaterials = new Dictionary<ColorType, Material>();

    public void Initialize()
    {
        _colorMaterials = GetSlicedItemSprite();
    }

    private Dictionary<ColorType, Material> GetSlicedItemSprite()
    {
        Dictionary<ColorType, Material> colorMaterials = new Dictionary<ColorType, Material>
        {
            [ColorType.White] = _whiteMaterial,
            [ColorType.Black] = _blackMaterial,
            [ColorType.Red] = _redMaterial,
            [ColorType.Yellow] = _yellowMaterial,
            [ColorType.Pink] = _pinkMaterial,
            [ColorType.Purple] = _purpleMaterial,

        };
        return colorMaterials;
    }
    public static Material GetColorMaterial(ColorType colorType)
    {
        return _colorMaterials[colorType];
    }
}
