using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class FireExtinguisher : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject sprayEffect;
    [SerializeField] private AudioSource spraySound;
    [SerializeField] private Transform sprayPoint;

    [Header("Spray Settings")]
    [SerializeField] private float sprayDistance = 5f;
    [SerializeField] private float extinguishRadius = 0.8f;
    [SerializeField] private float extinguishPower = 1.2f;
    [SerializeField] private LayerMask fireLayer = -1;

    [Header("Oil Settings")]
    [SerializeField] private Material powderCoveredMaterial;
    [SerializeField] private float coverSpeed = 0.5f;

    [Header("Haptic Feedback")]
    [SerializeField] private float hapticIntensity = 0.3f;
    [SerializeField] private float hapticDuration = 0.1f;


    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInputInteractor currentController;
    private bool isSpraying = false;
    private bool isGrabbed = false;

    void Awake()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
    }

    void Start()
    {
        if (sprayEffect != null)
        {
            sprayEffect.SetActive(false);
        }

        if (spraySound != null)
        {
            spraySound.playOnAwake = false;
            spraySound.loop = true;
            spraySound.Stop();
        }
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

        StopSpraying();
    }

    void Update()
    {
        if (isSpraying && isGrabbed)
        {
            SprayOnOil();
            SendHapticFeedback();
        }
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        isGrabbed = true;
        currentController = args.interactorObject as UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInputInteractor;
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        isGrabbed = false;
        StopSpraying();
        currentController = null;
    }

    private void OnActivated(ActivateEventArgs args)
    {
        if (!isGrabbed) return;
        StartSpraying();
    }

    private void OnDeactivated(DeactivateEventArgs args)
    {
        StopSpraying();
    }

    private void StartSpraying()
    {
        if (isSpraying) return;

        isSpraying = true;

        if (sprayEffect != null)
        {
            sprayEffect.SetActive(true);
        }

        if (spraySound != null && spraySound.clip != null)
        {
            spraySound.Play();
        }
    }

    private void StopSpraying()
    {
        if (!isSpraying) return;

        isSpraying = false;
        if (sprayEffect != null)
        {
            sprayEffect.SetActive(false);
        }

        if (spraySound != null && spraySound.isPlaying)
        {
            spraySound.Stop();
        }
    }

    private void SprayOnOil()
    {
        if (sprayPoint == null) return;

        RaycastHit[] hits = Physics.SphereCastAll(
            sprayPoint.position,
            extinguishRadius,
            sprayPoint.forward,
            sprayDistance,
            fireLayer
        );

        foreach (RaycastHit hit in hits)
        {
            Oil oil = hit.collider.GetComponent<Oil>();
            if (oil != null)
            {
                oil.ApplySpray(Time.deltaTime);
            }
        }
    }

    private void SendHapticFeedback()
    {
        if (currentController != null)
        {
            currentController.SendHapticImpulse(hapticIntensity, hapticDuration);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (sprayPoint == null) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(sprayPoint.position, sprayPoint.forward * sprayDistance);

        Gizmos.color = new Color(0, 1, 1, 0.3f);
        Gizmos.DrawWireSphere(sprayPoint.position, extinguishRadius);
        Gizmos.DrawWireSphere(sprayPoint.position + sprayPoint.forward * sprayDistance, extinguishRadius);
    }
}
