using UnityEngine;

public class FireExtinguisherCheck : MonoBehaviour
{  
    [SerializeField] private AudioClip soundUI;
    [SerializeField] private AudioSource audioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Binhcuuhoa"))
        {
            GameManage.Instance.SetActiveByIndex(13);
            audioSource.PlayOneShot(soundUI);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
