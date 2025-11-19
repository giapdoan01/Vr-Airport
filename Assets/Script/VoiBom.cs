using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class VoiBom : MonoBehaviour
{
    private bool hasGrabbed = false;
    public GameObject Checkponit;
    public AudioClip soundui5;
    private void OnEnable()
    {
        // Đăng ký sự kiện grab
        GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>().selectEntered.AddListener(OnGrab);
    }

    private void OnDisable()
    {
        // Hủy đăng ký khi object bị disable
        GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>().selectEntered.RemoveListener(OnGrab);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        if (!hasGrabbed)
        {
            hasGrabbed = true;
            DoOnce();
        }
    }

    private void DoOnce()
    {
        GameManage.Instance.PlayUiSound(soundui5);
        GameManage.Instance.SetActiveByIndex(4);
       Checkponit.SetActive(true);
        // Thực hiện logic ở đây, ví dụ: kích hoạt máy bơm, phát tiếng, v.v.
    }
}
