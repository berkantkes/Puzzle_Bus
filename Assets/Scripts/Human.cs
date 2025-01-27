using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Human : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    private ColorType _colorType = ColorType.Red;
    private string _idleAnimationTriggerName = "Idle";
    private string _runAnimationTriggerName = "Run";

    public ColorType ColorType => _colorType;

    public void MoveAlongPath(List<Vector2Int> path, GridManager gridManager, float durationPerStep = 0.5f)
    {
        _animator.SetInteger("RunInt", _animator.GetInteger("RunInt") + 1);

        Sequence moveSequence = DOTween.Sequence();
        foreach (Vector2Int gridPos in path)
        {
            Vector3 targetWorldPos = gridManager.GetWorldPosition(gridPos);

            moveSequence.Append(
                transform.DOMove(targetWorldPos, durationPerStep)
                    .SetEase(Ease.Linear)
                    .OnUpdate(() => RotateTowards(targetWorldPos))
            );
        }

        moveSequence.OnComplete(() =>
            _animator.SetInteger("RunInt", _animator.GetInteger("RunInt") - 1));
    }

    public void MoveToPosition(Vector3 targetPosition, float duration = 1f)
    {
        _animator.SetInteger("RunInt", _animator.GetInteger("RunInt") + 1);

        transform.DOMove(targetPosition, duration)
            .SetEase(Ease.Linear)
            .OnUpdate(() => RotateTowards(targetPosition))
            .OnComplete(() =>
                _animator.SetInteger("RunInt", _animator.GetInteger("RunInt") - 1)); // Callback when the movement completes
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
}
