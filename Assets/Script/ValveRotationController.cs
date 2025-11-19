using UnityEngine;

public class ValveRotationController : MonoBehaviour
{
    public Transform valve; // Valve cần xoay

    private Transform currentHandTransform;
    private Vector3 startHandDirection;
    private float startZRotation;

    public AudioClip SoundUi4;
    public AudioClip effectvalve;
    private bool hasPlayed = false;
    public void PlayUi4()
    {
        GameManage.Instance.SetActiveByIndex(3);
        GameManage.Instance.PlayUiSound(SoundUi4);
        GameManage.Instance.PlayEffectSound(effectvalve);



    }
    void OnEnable()
    {
        GrabPointEvents.OnGrabStart += OnGrabStart;
        GrabPointEvents.OnGrabEnd += OnGrabEnd;
    }

    void OnDisable()
    {
        GrabPointEvents.OnGrabStart -= OnGrabStart;
        GrabPointEvents.OnGrabEnd -= OnGrabEnd;
    }

    void Update()
    {
        if (currentHandTransform != null)
        {
            Vector3 currentDirection = (currentHandTransform.position - valve.position).normalized;
            float angleDelta = Vector3.SignedAngle(startHandDirection, currentDirection, valve.forward); // Xoay quanh Z

            float rawZ = startZRotation + angleDelta;

            // CHỈ CHO XOAY THEO CHIỀU ÂM (quay trái)
            if (angleDelta < 0)
            {
                float clampedZ = Mathf.Clamp(rawZ, -90f, 0f);
                float finalZ = clampedZ + 360f;

                valve.localEulerAngles = new Vector3(
                    valve.localEulerAngles.x,
                    valve.localEulerAngles.y,
                    finalZ
                );

                // Nếu đã xoay đến gần -90 độ (tức 270 độ Euler), và chưa phát
                if (!hasPlayed && Mathf.Abs(clampedZ + 90f) < 1f) // kiểm tra gần -90
                {
                    hasPlayed = true;
                    PlayUi4();
                }
            }
        }
    }

    void OnGrabStart(Transform handTransform)
    {
        currentHandTransform = handTransform;
        startHandDirection = (handTransform.position - valve.position).normalized;
        startZRotation = valve.localEulerAngles.z;

        // Convert từ 270–360 về -90 đến 0 (nếu cần)
        if (startZRotation > 180f)
            startZRotation -= 360f;
    }

    void OnGrabEnd(Transform handTransform)
    {
        if (currentHandTransform == handTransform)
        {
            currentHandTransform = null;
        }
    }
}
