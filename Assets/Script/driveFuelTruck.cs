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
    
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInputInteractor currentController;
    private bool isGrabbed = false;
    private bool isReversing = false;
    private float lastReverseTime = 0f;

    void Awake()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
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
                // Lùi liên tục
                truckController.ReverseTruck();
                SendHapticFeedback();
            }
            else
            {
                // Lùi theo interval
                if (Time.time - lastReverseTime >= reverseInterval)
                {
                    truckController.ReverseTruck();
                    SendHapticFeedback();
                    lastReverseTime = Time.time;
                }
            }
        }
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        isGrabbed = true;
        currentController = args.interactorObject as UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInputInteractor;
        StartReversing();
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        isGrabbed = false;
        StopReversing();
        currentController = null;
    }

    private void OnActivated(ActivateEventArgs args)
    {
        if (!isGrabbed) return;
        // Có thể thêm chức năng khác khi activate (ví dụ: tăng tốc)
        Debug.Log("Steering wheel activated");
    }

    private void OnDeactivated(DeactivateEventArgs args)
    {
        // Có thể thêm chức năng khác khi deactivate
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
}
