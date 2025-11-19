using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collider6 : MonoBehaviour
{
    public AudioClip soundUi6;
    public GameObject robe;
   // public GameObject checkPoint;
    public GameObject ladder_hide;
    public GameObject ladder_show;
    public GameObject head;

    // Start is called before the first frame update
    void Start()
    {

        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Voibom"))
        {
            Debug.Log(11);
            GameManage.Instance.PlayUiSound(soundUi6);
            robe.SetActive(false);
           // checkPoint.SetActive(false);
            ladder_hide.SetActive(false);
            ladder_show.SetActive(true);
           // head.SetActive(false) ;
            GameManage.Instance.SetActiveByIndex(5);

        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Voibom"))
        {
            Debug.Log(11);
            GameManage.Instance.PlayUiSound(soundUi6);
            robe.SetActive(false);
            // checkPoint.SetActive(false);
            ladder_hide.SetActive(false);
            ladder_show.SetActive(true);
            GameManage.Instance.SetActiveByIndex(5);

        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
