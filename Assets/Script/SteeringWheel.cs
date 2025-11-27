using UnityEngine;

public class SteeringWheelController : MonoBehaviour
{
    [Header("References")]
    public Transform steeringWheel; // Vô lăng cần xoay

    [Header("Rotation Settings")]
    [SerializeField] private float maxRotation = 450f; // Xoay tối đa (trái + phải)
    [SerializeField] private bool limitRotation = true; // Giới hạn xoay
    
    [Header("Audio")]
    public AudioClip rotationSound;
    
    [Header("Debug")]
    [SerializeField] private bool showDebug = true;
    
    private Transform currentHandTransform;
    private float startAngle; // Góc ban đầu khi grab
    private float currentRotation = 0f;
    
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
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
            Vector3 localHandPos = steeringWheel.InverseTransformPoint(currentHandTransform.position);
            
            float currentAngle = Mathf.Atan2(localHandPos.y, localHandPos.x) * Mathf.Rad2Deg;
            
            float deltaAngle = Mathf.DeltaAngle(startAngle, currentAngle);
            
            currentRotation += deltaAngle;
            
            if (limitRotation)
            {
                currentRotation = Mathf.Clamp(currentRotation, -maxRotation, maxRotation);
            }
            
            startAngle = currentAngle;
            
            steeringWheel.localEulerAngles = new Vector3(
                steeringWheel.localEulerAngles.x,
                steeringWheel.localEulerAngles.y,
                -currentRotation
            );
            
        }
    }

    void OnGrabStart(Transform handTransform)
    {
        currentHandTransform = handTransform;
        

        Vector3 localHandPos = steeringWheel.InverseTransformPoint(handTransform.position);
        startAngle = Mathf.Atan2(localHandPos.y, localHandPos.x) * Mathf.Rad2Deg;
        
        if (audioSource != null && rotationSound != null)
        {
            audioSource.PlayOneShot(rotationSound);
        }
        
        if (showDebug)
        {
            Debug.Log($"Grabbed steering wheel at angle: {startAngle:F1}°");
        }
    }

    void OnGrabEnd(Transform handTransform)
    {
        if (currentHandTransform == handTransform)
        {
            currentHandTransform = null;
            
            if (showDebug)
            {
                Debug.Log($"Released steering wheel at rotation: {currentRotation:F1}°");
            }
        }
    }

    public float GetSteeringAngle()
    {
        return currentRotation;
    }

    public float GetSteeringInput()
    {
        return currentRotation / maxRotation;
    }


    public void ResetSteering()
    {
        currentRotation = 0f;
        steeringWheel.localEulerAngles = new Vector3(
            steeringWheel.localEulerAngles.x,
            steeringWheel.localEulerAngles.y,
            0f
        );
    }

    // Gizmos để debug
    void OnDrawGizmosSelected()
    {
        if (steeringWheel == null) return;
        
        // Vẽ center
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(steeringWheel.position, 0.05f);
        
        // Vẽ trục Z (forward)
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(steeringWheel.position, steeringWheel.forward * 0.2f);
        
        // Vẽ max rotation (quanh trục Z)
        Gizmos.color = Color.red;
        Vector3 leftMax = Quaternion.AngleAxis(-maxRotation, steeringWheel.forward) * steeringWheel.right;
        Vector3 rightMax = Quaternion.AngleAxis(maxRotation, steeringWheel.forward) * steeringWheel.right;
        Gizmos.DrawRay(steeringWheel.position, leftMax * 0.3f);
        Gizmos.DrawRay(steeringWheel.position, rightMax * 0.3f);
    }
}
