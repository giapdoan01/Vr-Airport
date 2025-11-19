using UnityEngine;


public class SteeringWheel : MonoBehaviour
{
    public Transform wheelCenter; // điểm trung tâm vô lăng (pivot)
    public float maxRotationAngle = 180f; // Góc tối đa quay trái/phải

    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor interactor;
    private Quaternion initialRotation;
    private float initialAngle;

    private void Start()
    {
        initialRotation = transform.localRotation;
    }

    public void OnGrab(UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor interactor)
    {
        this.interactor = interactor;
        Vector3 localGrabPos = wheelCenter.InverseTransformPoint(interactor.transform.position);
        initialAngle = Mathf.Atan2(localGrabPos.y, localGrabPos.x) * Mathf.Rad2Deg;
    }

    public void OnRelease()
    {
        interactor = null;
    }

    private void Update()
    {
        if (interactor != null)
        {
            Vector3 localGrabPos = wheelCenter.InverseTransformPoint(interactor.transform.position);
            float currentAngle = Mathf.Atan2(localGrabPos.y, localGrabPos.x) * Mathf.Rad2Deg;

            float deltaAngle = Mathf.DeltaAngle(initialAngle, currentAngle);
            float newAngle = Mathf.Clamp(deltaAngle, -maxRotationAngle, maxRotationAngle);

            transform.localRotation = initialRotation * Quaternion.Euler(0, 0, -newAngle);
        }
    }
}
