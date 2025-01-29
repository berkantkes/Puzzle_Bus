using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Human : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Renderer _renderer;
    [SerializeField] private Material _outlineMaterial;

    private Material[] _originalMaterials;
    private Material[] _outlineMaterials;
    private ColorType _colorType;
    private bool _hasItMoved = false;
    private string _idleAnimationTriggerName = "Idle";
    private string _runAnimationTriggerName = "Run";
    private int _isMoving = 0;
    public int IsMoving => _isMoving;

    public ColorType ColorType => _colorType;
    
    public void Initialize(ColorType colorType)
    {
        _originalMaterials = new Material[1];
        _originalMaterials[0] = ColorMaterialSelector.GetColorMaterial(colorType);

        _outlineMaterials = new Material[2];
        _outlineMaterials[0] = _originalMaterials[0];
        _outlineMaterials[1] = _outlineMaterial;     

        _renderer.materials = _originalMaterials; 
        _colorType = colorType;           
    }
    
    public void MoveAlongPath(List<Vector2Int> path, GridManager gridManager, float speed = 5f)
    {
        _animator.SetInteger("RunInt", _animator.GetInteger("RunInt") + 1);
        _isMoving++;
        _hasItMoved = true;

        Sequence moveSequence = DOTween.Sequence();
    
        foreach (Vector2Int gridPos in path)
        {
            Vector3 targetWorldPos = gridManager.GetWorldPosition(gridPos);
            float distance = Vector3.Distance(transform.position, targetWorldPos);
            float duration = distance / speed; // Hızı sabit tutarak sürenin mesafeye göre hesaplanması

            moveSequence.Append(
                transform.DOMove(targetWorldPos, .15f)
                    .SetEase(Ease.Linear)
                    .OnUpdate(() => RotateTowards(targetWorldPos))
            );
        }

        moveSequence.OnComplete(() =>
        {
            _animator.SetInteger("RunInt", _animator.GetInteger("RunInt") - 1);
            _isMoving--;
        });
    }

    public void MoveToPosition(Vector3 targetPosition, float speed = 7f)
    {
        _animator.SetInteger("RunInt", _animator.GetInteger("RunInt") + 1);
        _isMoving++;
        _hasItMoved = true;

        float distance = Vector3.Distance(transform.position, targetPosition);
        float duration = distance / speed; // Süre = Mesafe / Hız

        transform.DOMove(targetPosition, duration)
            .SetEase(Ease.Linear)
            .OnUpdate(() => RotateTowards(targetPosition))
            .OnComplete(() =>
            {
                _animator.SetInteger("RunInt", _animator.GetInteger("RunInt") - 1);
                _isMoving--;
            });
    }


    private void RotateTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(-direction);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    public void UpdateMaterial(bool outline)
    {
        if (outline && !_hasItMoved)
            _renderer.materials = _outlineMaterials;
        else
            _renderer.materials = _originalMaterials;
    }

    public void ClearHuman()
    {
        _renderer.materials = _originalMaterials;
        _hasItMoved = false;
    }
}
