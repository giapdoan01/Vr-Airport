using UnityEngine;

public class checkElectricRope : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("ElectricRope"))
        {
            GameManage.Instance.SetActiveByIndex(17);

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
