using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class driveFuelTruck : MonoBehaviour
{
    [Header("Truck Reference")]
    [SerializeField] private fuelTruck truckController;
    
    [Header("Control Settings")]
    [SerializeField] private bool continuousReverse = true;
    [SerializeField] private float reverseInterval = 0.1f;
    
    [Header("Haptic Feedback")]
    [SerializeField] private float hapticIntensity = 0.2f;
    [SerializeField] private float hapticDuration = 0.1f;
    
    [Header("Steering Wheel Settings")]
    [SerializeField] private bool allowRotation = true;
    [SerializeField] private bool lockRotationXY = true;
    
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInputInteractor currentController;
    private bool isGrabbed = false;
    private bool isReversing = false;
    private float lastReverseTime = 0f;
    
    // Lưu parent và local transform
    private Transform originalParent;
    private Vector3 lockedLocalPosition;
    private Quaternion lockedLocalRotation;
    private Vector3 lockedLocalScale;

    void Awake()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        
        // Lưu parent và transform ban đầu
        originalParent = transform.parent;
        lockedLocalPosition = transform.localPosition;
        lockedLocalRotation = transform.localRotation;
        lockedLocalScale = transform.localScale;
        
        // Force settings cho XRGrabInteractable
        if (grabInteractable != null)
        {
            grabInteractable.trackPosition = false;
            grabInteractable.trackRotation = true;
            grabInteractable.throwOnDetach = false;
            grabInteractable.retainTransformParent = true;
            grabInteractable.trackScale = false;
        }
    }

    void Start()
    {
        
    }

    void OnEnable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrabbed);
            grabInteractable.selectExited.AddListener(OnReleased);
            grabInteractable.activated.AddListener(OnActivated);
            grabInteractable.deactivated.AddListener(OnDeactivated);
        }
    }

    void OnDisable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
            grabInteractable.selectExited.RemoveListener(OnReleased);
            grabInteractable.activated.RemoveListener(OnActivated);
            grabInteractable.deactivated.RemoveListener(OnDeactivated);
        }

        StopReversing();
    }

    void Update()
    {
        if (isReversing && isGrabbed && truckController != null)
        {
            if (continuousReverse)
            {
                truckController.ReverseTruck();
                SendHapticFeedback();
            }
            else
            {
                if (Time.time - lastReverseTime >= reverseInterval)
                {
                    truckController.ReverseTruck();
                    SendHapticFeedback();
                    lastReverseTime = Time.time;
                }
            }
        }
    }
    
    void LateUpdate()
    {
        // Force giữ parent
        if (transform.parent != originalParent)
        {
            transform.SetParent(originalParent);
            Debug.LogWarning("Steering wheel parent was changed! Restoring to: " + originalParent.name);
        }
        
        // LUÔN lock position và scale
        transform.localPosition = lockedLocalPosition;
        transform.localScale = lockedLocalScale;
        
        // ===== CHỈ lock rotation X, Y KHI ĐANG GRAB =====
        if (isGrabbed && lockRotationXY)
        {
            Vector3 currentEuler = transform.localEulerAngles;
            transform.localRotation = Quaternion.Euler(
                lockedLocalRotation.eulerAngles.x, // Lock X
                lockedLocalRotation.eulerAngles.y, // Lock Y
                currentEuler.z                      // Cho phép Z
            );
        }
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        isGrabbed = true;
        currentController = args.interactorObject as UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInputInteractor;
        
        // Đảm bảo parent đúng
        if (transform.parent != originalParent)
        {
            transform.SetParent(originalParent);
        }
        
        // Force scale ngay khi grab
        transform.localScale = lockedLocalScale;
        
        StartReversing();
        
        Debug.Log($"Grabbed steering wheel - Parent: {transform.parent.name}, Scale: {transform.localScale}");
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        isGrabbed = false;
        StopReversing();
        currentController = null;
        
        // Đảm bảo parent đúng
        if (transform.parent != originalParent)
        {
            transform.SetParent(originalParent);
        }
        
        // Force scale khi thả
        transform.localScale = lockedLocalScale;
        
        Debug.Log($"Released steering wheel - Parent: {transform.parent.name}, Scale: {transform.localScale}");
    }

    private void OnActivated(ActivateEventArgs args)
    {
        if (!isGrabbed) return;
        Debug.Log("Steering wheel activated");
    }

    private void OnDeactivated(DeactivateEventArgs args)
    {
        Debug.Log("Steering wheel deactivated");
    }

    private void StartReversing()
    {
        if (isReversing) return;
        
        isReversing = true;
        lastReverseTime = Time.time;
    }

    private void StopReversing()
    {
        if (!isReversing) return;
        
        isReversing = false;
    }

    private void SendHapticFeedback()
    {
        if (currentController != null)
        {
            currentController.SendHapticImpulse(hapticIntensity, hapticDuration);
        }
    }

    public void ManualReverse()
    {
        if (truckController != null)
        {
            truckController.ReverseTruck();
        }
    }

    public void ManualReverseForDuration(float duration)
    {
        if (truckController != null)
        {
            truckController.ReverseTruckForDuration(duration);
        }
    }
    
    public bool IsParentCorrect()
    {
        return transform.parent == originalParent;
    }
    
    public void ForceRestoreParent()
    {
        if (transform.parent != originalParent)
        {
            transform.SetParent(originalParent);
            transform.localPosition = lockedLocalPosition;
            transform.localRotation = lockedLocalRotation;
            transform.localScale = lockedLocalScale;
            Debug.Log("Force restored parent to: " + originalParent.name);
        }
    }
    
    public float GetSteeringAngle()
    {
        return transform.localEulerAngles.z;
    }
    
    public float GetSteeringInput()
    {
        float angle = transform.localEulerAngles.z;
        
        if (angle > 180f)
            angle -= 360f;
        
        float maxAngle = 450f;
        return Mathf.Clamp(angle / maxAngle, -1f, 1f);
    }
}
