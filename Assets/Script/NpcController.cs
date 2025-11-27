using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class NpcController : MonoBehaviour
{
    [Header("References")]
    public AIDestinationSetter Destination;
    public Animator ani;

    [Header("Timer Settings")]
    [SerializeField] private float runTime = 5f; // Thời gian chạy (giây)
    
    private Coroutine idleCoroutine;
    private bool isRunning = false;

    public void WarnNpc()
    {
        if (idleCoroutine != null)
        {
            StopCoroutine(idleCoroutine);
        }
        
        Destination.enabled = true;
        ani.Play("run");
        isRunning = true;
        
        idleCoroutine = StartCoroutine(ReturnToIdleAfterTime());
    }

    private IEnumerator ReturnToIdleAfterTime()
    {
        yield return new WaitForSeconds(runTime);
        
        ReturnToIdle();
    }

    private void ReturnToIdle()
    {
        Destination.enabled = false;
        
        ani.Play("idle");
        
        isRunning = false;
        
    }

    public void ForceIdle()
    {
        if (idleCoroutine != null)
        {
            StopCoroutine(idleCoroutine);
        }
        
        ReturnToIdle();
    }

    public bool IsRunning()
    {
        return isRunning;
    }

    void OnDisable()
    {
        if (idleCoroutine != null)
        {
            StopCoroutine(idleCoroutine);
        }
    }
}
