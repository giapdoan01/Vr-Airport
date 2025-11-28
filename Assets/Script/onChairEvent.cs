using UnityEngine;

public class onChairEvent : MonoBehaviour
{
    [Header("Player Reference")]
    [SerializeField] private GameObject player;
    
    [Header("Target Position")]
    [SerializeField] private Vector3 targetPosition = new Vector3(-18.53978f, 0.934f, -13.05468f);
    [SerializeField] private Vector3 targetRotation = new Vector3(-180f, 88.76f, 180f);
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }
    
    // Dịch chuyển player đến vị trí ghế
    public void SitOnChair()
    {
        if (player == null)
        {
            Debug.LogError("Player chưa được gán!");
            return;
        }
        
        // Set position
        player.transform.position = targetPosition;
        
        // Set rotation
        player.transform.rotation = Quaternion.Euler(targetRotation);
        
        Debug.Log($"Player đã dịch chuyển đến: {targetPosition}");
    }
}
