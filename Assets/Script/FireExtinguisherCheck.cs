using UnityEngine;

public class FireExtinguisherCheck : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Binhcuuhoa"))
        {
            GameManage.Instance.SetActiveByIndex(13);

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
