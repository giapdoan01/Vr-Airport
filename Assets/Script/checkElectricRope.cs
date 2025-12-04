using UnityEngine;

public class checkElectricRope : MonoBehaviour
{
    [SerializeField] private AudioClip audioClipTrue;
    [SerializeField] private AudioClip audioClipFalse;
    [SerializeField] private AudioSource audioSource;
    void Start()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("ElectricRope"))
        {
            if (GameManage.Instance.isMore4m2)
            {
                GameManage.Instance.SetActiveByIndex(19);
                audioSource.PlayOneShot(audioClipTrue);
            }
            else
            {
                GameManage.Instance.SetActiveByIndex(26);
                audioSource.PlayOneShot(audioClipFalse);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
