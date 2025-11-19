using System.Collections;
using UnityEngine;

public class Collder9 : MonoBehaviour
{
    private Coroutine mopCoroutine = null;
    public AudioClip soundui10;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Mop"))
        {
            // Bắt đầu đếm thời gian nếu chạm vào object có tag "mop"
            mopCoroutine = StartCoroutine(CheckMopCollision());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Mop"))
        {
            // Dừng coroutine nếu rời khỏi object "mop" trước khi đủ 4 giây
            if (mopCoroutine != null)
            {
                StopCoroutine(mopCoroutine);
                mopCoroutine = null;
            }
        }
    }

    private IEnumerator CheckMopCollision()
    {
        yield return new WaitForSeconds(4f);
       GameManage.Instance.SetActiveByIndex(9);
        GameManage.Instance.PlayUiSound(soundui10);
    }
}
