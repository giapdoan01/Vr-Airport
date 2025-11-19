using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_valve : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator ani;

    public void Playani(string temp)
    {
        ani.Play(temp);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
