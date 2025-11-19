using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class NpcController : MonoBehaviour
{
    public AIDestinationSetter Destination;
    public Animator ani;
    public GameObject player;
    public float activeDistance = 10f; // Khoảng cách để hiện nút
    public GameObject buttonWarn;

    private void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

       
    }

    private void Update()
    {
       
    }

    public void WarnNpc()
    {
        Destination.enabled = true;
        ani.Play("run");
    }
}
