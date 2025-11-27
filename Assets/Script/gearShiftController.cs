using UnityEngine;

public class GearShiftController : MonoBehaviour
{
    [Header("References")]
    public Transform gearShift; // Cần số

    [Header("Rotation Settings")]
    [SerializeField] private float maxRotation = 90f; // Góc xoay max
    [SerializeField] private bool limitRotation = true;
    
    [Header("Debug")]
    [SerializeField] private bool showDebug = true;
    
    private Transform currentHandTransform;
    private float startAngle;
    private float currentRotation = 0f;
    
    // Lock transform
    private Vector3 lockedPosition;
    private Quaternion lockedRotation;

    void Start()
    {
        lockedPosition = gearShift.position;
        Vector3 euler = gearShift.localEulerAngles;
        lockedRotation = Quaternion.Euler(euler.x, euler.y, 0);
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
            // Chuyển vị trí tay về local space
            Vector3 localHandPos = gearShift.InverseTransformPoint(currentHandTransform.position);
            
            // ===== THAY ĐỔI: Tính góc trong mặt phẳng XZ (thay vì XY) =====
            // Vì trục Z nằm ngang, tay di chuyển lên/xuống (trục Y)
            float currentAngle = Mathf.Atan2(localHandPos.y, localHandPos.x) * Mathf.Rad2Deg;
            
            // Tính delta angle
            float deltaAngle = Mathf.DeltaAngle(startAngle, currentAngle);
            
            // Cộng vào current rotation
            currentRotation += deltaAngle;
            
            // Giới hạn rotation
            if (limitRotation)
            {
                currentRotation = Mathf.Clamp(currentRotation, -maxRotation, maxRotation);
            }
            
            // Update start angle
            startAngle = currentAngle;
            
            if (showDebug)
            {
                Debug.Log($"Gear Shift Rotation: {currentRotation:F1}°");
            }
        }
    }

    void LateUpdate()
    {
        // Lock position
        gearShift.position = lockedPosition;
        
        // Apply rotation quanh trục Z
        Vector3 euler = lockedRotation.eulerAngles;
        gearShift.localRotation = Quaternion.Euler(euler.x, euler.y, -currentRotation);
    }

    void OnGrabStart(Transform handTransform)
    {
        currentHandTransform = handTransform;
        
        Vector3 localHandPos = gearShift.InverseTransformPoint(handTransform.position);
        startAngle = Mathf.Atan2(localHandPos.y, localHandPos.x) * Mathf.Rad2Deg;
        
        if (showDebug)
        {
            Debug.Log($"Grabbed gear shift at angle: {startAngle:F1}°");
        }
    }

    void OnGrabEnd(Transform handTransform)
    {
        if (currentHandTransform == handTransform)
        {
            currentHandTransform = null;
            
            if (showDebug)
            {
                Debug.Log($"Released gear shift at rotation: {currentRotation:F1}°");
            }
        }
    }

    public float GetGearRotation()
    {
        return currentRotation;
    }

    void OnDrawGizmosSelected()
    {
        if (gearShift == null) return;
        
        // Vẽ center
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(gearShift.position, 0.05f);
        
        // Vẽ trục Z (forward) - Nằm ngang
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(gearShift.position, gearShift.forward * 0.2f);
        
        // Vẽ max rotation
        Gizmos.color = Color.red;
        Vector3 upMax = Quaternion.AngleAxis(-maxRotation, gearShift.forward) * gearShift.up;
        Vector3 downMax = Quaternion.AngleAxis(maxRotation, gearShift.forward) * gearShift.up;
        Gizmos.DrawRay(gearShift.position, upMax * 0.3f);
        Gizmos.DrawRay(gearShift.position, downMax * 0.3f);
    }
}
