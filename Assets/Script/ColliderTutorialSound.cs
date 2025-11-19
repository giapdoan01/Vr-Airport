using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderTutorialSound : MonoBehaviour
{
    public AudioClip[] audioClips;
    public int index;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManage.Instance.PlayUiSound(audioClips[index]);
            Destroy(gameObject);
        }
    }
}
