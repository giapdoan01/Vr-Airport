using UnityEngine;

public class checkFireTruck : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("truck"))
        {
            GameManage.Instance.SetActiveByIndex(24);

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

