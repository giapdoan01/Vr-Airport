using UnityEngine;

public class UIFollowPlayer : MonoBehaviour
{
    [Header("Player Camera")]
    public Transform playerCamera;

    [Header("Fixed Offset Settings")]
    public Vector3 fixedOffset = new Vector3(0, 1.5f, 1.0f); // Vị trí cố định so với hướng ban đầu của người chơi

    [Header("Rotation Settings")]
    public bool matchCameraRotation = false;  // Có xoay theo camera không

    void LateUpdate()
    {
        if (playerCamera == null) return;

        // Giữ UI ở vị trí cố định so với player
        transform.position = playerCamera.position + playerCamera.TransformDirection(fixedOffset);

        // Không di chuyển UI ra trước mặt mới nếu người chơi xoay
        if (matchCameraRotation)
        {
            transform.rotation = playerCamera.rotation;
        }
    }
}
