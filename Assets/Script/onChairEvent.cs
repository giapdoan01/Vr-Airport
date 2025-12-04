using UnityEngine;

public class onChairEvent : MonoBehaviour
{
    [Header("Player Reference")]
    [SerializeField] private GameObject player;
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }
    
    // Dịch chuyển player đến vị trí của object này
    public void SitOnChair()
    {
        if (player == null)
        {
            Debug.LogError("Player chưa được gán!");
            return;
        }
        
        // Chỉ set position = position của object này
        player.transform.position = transform.position;
        
        // ❌ KHÔNG đổi rotation
        // ❌ KHÔNG đổi scale
        
        Debug.Log($"Player đã dịch chuyển đến: {transform.position}");
    }
}
