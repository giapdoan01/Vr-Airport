using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GrabPointEvents : MonoBehaviour
{
    public static System.Action<Transform> OnGrabStart;
    public static System.Action<Transform> OnGrabEnd;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable interactable;

    void Awake()
    {
        interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
    }

    void OnEnable()
    {
        interactable.selectEntered.AddListener(OnSelectEnter);
        interactable.selectExited.AddListener(OnSelectExit);
    }

    void OnDisable()
    {
        interactable.selectEntered.RemoveListener(OnSelectEnter);
        interactable.selectExited.RemoveListener(OnSelectExit);
    }

    void OnSelectEnter(SelectEnterEventArgs args)
    {
        OnGrabStart?.Invoke(args.interactorObject.transform); // Lấy Transform của tay
    }

    void OnSelectExit(SelectExitEventArgs args)
    {
        OnGrabEnd?.Invoke(args.interactorObject.transform);
    }
}
