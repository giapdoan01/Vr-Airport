using UnityEngine;

public class truckCheck : MonoBehaviour
{
    [SerializeField] private AudioClip soundUI;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameObject truck;
    private fuelTruck fuelTruck;
    void Start()
    {
        if (truck != null)
        {
            fuelTruck = truck.GetComponent<fuelTruck>();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManage.Instance.SetActiveByIndex(22);
            audioSource.PlayOneShot(soundUI);
            fuelTruck.reverseSpeed = 0f;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
