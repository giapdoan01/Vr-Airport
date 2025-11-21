using UnityEngine;

public class BarrierCheck : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Barrier"))
        {
            GameManage.Instance.SetActiveByIndex(15);

        }
    }
    void Update()
    {
        
    }
}
