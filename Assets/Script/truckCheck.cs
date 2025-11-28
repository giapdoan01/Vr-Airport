using UnityEngine;

public class truckCheck : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManage.Instance.SetActiveByIndex(22);

        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
