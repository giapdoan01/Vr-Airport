using UnityEngine;

public class checkFireTruck : MonoBehaviour
{
    [SerializeField] private AudioClip soundUI;
    [SerializeField] private AudioSource audioSource;
    void Start()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("truck"))
        {
            GameManage.Instance.SetActiveByIndex(24);
            audioSource.PlayOneShot(soundUI);

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

