using UnityEngine;
using DG.Tweening;

public class GateTransition : MonoBehaviour
{
    [Header("Gate References")]
    public Transform leftGate;
    public Transform rightGate;

    [Header("Transition Settings")]
    public float moveDuration = 1f;
    public float delayBetween = 0.3f;
    public float rotationDuration = 0.5f;

    private Vector3 leftGateInitialPos;
    private Vector3 rightGateInitialPos;

    void Start()
    {
        // Store the initial local positions
        leftGateInitialPos = leftGate.localPosition;
        rightGateInitialPos = rightGate.localPosition;

    }

    public void TriggerTransition(GameObject before, GameObject after)
    {
        // Rotate this object randomly on Z
        float randomAngle = Random.Range(0, 360);
        transform.DORotate(transform.eulerAngles + new Vector3(0f, 0f, randomAngle), rotationDuration, RotateMode.FastBeyond360)
                 .SetEase(Ease.OutSine);

        // Animate gates to center (Y = 0), then back to initial positions
        Sequence gateSequence = DOTween.Sequence();

        // Move both gates to Y = 0 (keep X and Z the same)
        gateSequence.Append(leftGate.DOLocalMoveY(0f, moveDuration).SetEase(Ease.InOutSine));
        gateSequence.Join(rightGate.DOLocalMoveY(0f, moveDuration).SetEase(Ease.InOutSine).OnComplete(() => {
            before.SetActive(false);
            after.SetActive(true);
        }));

        // Pause before returning
        gateSequence.AppendInterval(delayBetween);

        // Move both gates back to their original local positions
        gateSequence.Append(leftGate.DOLocalMoveY(leftGateInitialPos.y, moveDuration).SetEase(Ease.InOutSine));
        gateSequence.Join(rightGate.DOLocalMoveY(rightGateInitialPos.y, moveDuration).SetEase(Ease.InOutSine));
    }
}
