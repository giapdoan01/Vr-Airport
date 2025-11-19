using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collider7 : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioClip soundUi8;
   

    // Start is called before the first frame update
    void Start()
    {


    }
   
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Thang"))
        {
            GameManage.Instance.PlayUiSound(soundUi8);
            GameManage.Instance.SetActiveByIndex(7);

        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}
