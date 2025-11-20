using UnityEngine;
using UnityEngine.UI;

public class ButtonCheck : MonoBehaviour
{
    [SerializeField] private GameObject button;
    [SerializeField] private GameObject buttonSwap;
    [SerializeField] private Button button1;
    [SerializeField] private Button buttonSwap1;
    void Start()
    {
        buttonSwap.SetActive(false);
        button1.onClick.AddListener(Select);
        buttonSwap1.onClick.AddListener(unSelect);

    }
    void Select()
    {
        button.SetActive(false);
        buttonSwap.SetActive(true);
    }
    void unSelect()
    {
        button.SetActive(true);
        buttonSwap.SetActive(false);
    }
    
    
}
