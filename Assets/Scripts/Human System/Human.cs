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
    private int _isMoving = 0;

    private const float DefaultSpeed = 5f;
    private const float BusSpeed = 7f;
    private const float RotationSpeed = 10f;

    public int IsMoving => _isMoving;
    public ColorType ColorType => _colorType;

    public void Initialize(ColorType colorType)
    {
        _colorType = colorType;
        _originalMaterials = new[] { ColorMaterialSelector.GetColorMaterial(colorType) };
        _outlineMaterials = new[] { _originalMaterials[0], _outlineMaterial };
        _renderer.materials = _originalMaterials;
    }

    public void MoveAlongPath(List<Vector2Int> path, GridManager gridManager)
    {
        StartMovement();
        Sequence moveSequence = DOTween.Sequence();

        foreach (Vector2Int gridPos in path)
        {
            Vector3 targetWorldPos = gridManager.GetWorldPosition(gridPos);
            float duration = CalculateMovementDuration(targetWorldPos, DefaultSpeed);
            float stepTime = .2f;

            moveSequence.Append(
                transform.DOMove(targetWorldPos, stepTime)
                    .SetEase(Ease.Linear)
                    .OnUpdate(() => RotateTowards(targetWorldPos))
            );
        }

        moveSequence.OnComplete(EndMovement);
    }

    public void MoveToBusStopPosition(Vector3 targetPosition)
    {
        MoveToTarget(targetPosition, BusSpeed);
    }

    public void MoveToBusPosition(Vector3 targetPosition, Bus bus)
    {
        transform.SetParent(bus.transform);
        MoveToTarget(targetPosition, BusSpeed);
    }

    private void MoveToTarget(Vector3 targetPosition, float speed)
    {
        StartMovement();
        float duration = CalculateMovementDuration(targetPosition, speed);

        transform.DOMove(targetPosition, duration)
            .SetEase(Ease.Linear)
            .OnUpdate(() => RotateTowards(targetPosition))
            .OnComplete(EndMovement);
    }

    private float CalculateMovementDuration(Vector3 targetPosition, float speed)
    {
        float distance = Vector3.Distance(transform.position, targetPosition);
        return distance / speed;
    }

    private void RotateTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(-direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * RotationSpeed);
        }
    }

    public void UpdateMaterial(bool outline)
    {
        _renderer.materials = outline && !_hasItMoved ? _outlineMaterials : _originalMaterials;
    }

    public void ClearHuman()
    {
        _renderer.materials = _originalMaterials;
        _hasItMoved = false;
    }

    private void StartMovement()
    {
        _animator.SetInteger("RunInt", _animator.GetInteger("RunInt") + 1);
        _isMoving++;
        _hasItMoved = true;
    }

    private void EndMovement()
    {
        _animator.SetInteger("RunInt", _animator.GetInteger("RunInt") - 1);
        _isMoving--;
    }
}
