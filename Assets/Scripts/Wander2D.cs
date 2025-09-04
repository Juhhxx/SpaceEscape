using UnityEngine;
using DG.Tweening;

public class Wander2D : MonoBehaviour
{
    public float rotationSpeed = 30f;         // Degrees per second
    public float moveRadius = 1.5f;           // Max distance from origin
    public float moveDuration = 2f;           // Duration of each move
    public float pauseBetweenMoves = 0.5f;    // Wait time between moves

    private Vector2 origin;

    void Start()
    {
        origin = transform.position;
        RotateForever();
        WanderLoop();
    }

    void RotateForever()
    {
        // Continuous rotation using manual rotation instead of tweening
        DOTween.To(() => 0f, x => transform.Rotate(0, 0, rotationSpeed * Time.deltaTime), 1f, Mathf.Infinity)
               .SetEase(Ease.Linear)
               .SetUpdate(true);
    }

    void WanderLoop()
    {
        Vector2 nextPosition = origin + Random.insideUnitCircle * moveRadius;

        transform.DOMove(nextPosition, moveDuration)
                 .SetEase(Ease.InOutSine)
                 .OnComplete(() =>
                 {
                     DOVirtual.DelayedCall(pauseBetweenMoves, WanderLoop);
                 });
    }
}
