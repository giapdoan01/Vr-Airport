using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class NpcController : MonoBehaviour
{
    public AIDestinationSetter Destination;
    public Animator ani;

    public void WarnNpc()
    {
        Destination.enabled = true;
        ani.Play("run");
    }
}
