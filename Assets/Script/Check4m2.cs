using UnityEngine;

public class Check4m2 : MonoBehaviour
{
    [Header("Button References")]
    [SerializeField] private GameObject panelReport;  // GameObject for button <4m2
    [SerializeField] private ButtonCheck buttonLessThan4m2;  // Button <4m2
    [SerializeField] private ButtonCheck buttonMoreThan4m2;  // Button >4m2

    void Update()
    {
        // Kiểm tra nếu button <4m2 được chọn
        if (buttonLessThan4m2 != null && buttonLessThan4m2.IsSelected())
        {
            GameManage.Instance.SetActiveByIndex(11);
            panelReport.SetActive(false);
        }
    }
}
