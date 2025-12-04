using UnityEngine;

public class Check4m2 : MonoBehaviour
{
    [SerializeField] private AudioClip soundUI;
    [SerializeField] private AudioSource audioSource;
    [Header("Button References")]
    [SerializeField] private GameObject panelReport;  // GameObject for button <4m2
    [SerializeField] private ButtonCheck buttonLessThan4m2;  // Button <4m2
    [SerializeField] private ButtonCheck buttonMoreThan4m2;  // Button >4m2

    void Update()
    {
        // Kiểm tra nếu button <4m2 được chọn
        if (buttonLessThan4m2 != null && buttonLessThan4m2.IsSelected())
        {
            // ✅ Set biến isMore4m2 = false (<4m2)
            GameManage.Instance.isMore4m2 = false;
            
            GameManage.Instance.SetActiveByIndex(11);
            audioSource.PlayOneShot(soundUI);
            panelReport.SetActive(false);
            
            Debug.Log("Chọn <4m2 → isMore4m2 = false");
        }
        
        // Kiểm tra nếu button >4m2 được chọn
        if (buttonMoreThan4m2 != null && buttonMoreThan4m2.IsSelected())
        {
            // ✅ Set biến isMore4m2 = true (>4m2)
            GameManage.Instance.isMore4m2 = true;
            
            GameManage.Instance.SetActiveByIndex(11);
            audioSource.PlayOneShot(soundUI);
            panelReport.SetActive(false);
            
            Debug.Log("Chọn >4m2 → isMore4m2 = true");
        }
    }
}
