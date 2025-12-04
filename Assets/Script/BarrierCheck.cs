using UnityEngine;

public class BarrierCheck : MonoBehaviour
{
    [SerializeField] private AudioClip soundUITrue;
    [SerializeField] private AudioClip soundUIFalse;
    [SerializeField] private AudioSource audioSource;
    
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Barrier"))
        {
            if (GameManage.Instance.isMore4m2)
            {
                GameManage.Instance.SetActiveByIndex(15);
                audioSource.PlayOneShot(soundUITrue);
            }
            else
            {
                GameManage.Instance.SetActiveByIndex(25);
                audioSource.PlayOneShot(soundUIFalse);
            }
            

        }
    }
    void Update()
    {
        
    }
}
