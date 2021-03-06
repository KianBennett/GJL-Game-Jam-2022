using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turbine : MonoBehaviour
{
    private TriggerObject trigger;
    private bool hasSteam;
    private Animator anim;
    private bool isAnimPlaying;
    public GameObject steamCol;
    public AudioSource AudioSource;
    // Start is called before the first frame update
    void Start()
    {
        trigger = GetComponent<TriggerObject>();
        hasSteam = false;
        anim = GetComponent<Animator>();
        isAnimPlaying = false;
        steamCol = steamCol = GameObject.Find("tempCol");
    }

    // Update is called once per frame
    void Update()
    {
        if(hasSteam)
        {
            trigger.active = true;
            if(!isAnimPlaying)
            {
                anim.SetTrigger("turbineOn");
                isAnimPlaying = true;
                steamCol = GameObject.FindGameObjectWithTag("Steam");
            }
            
        }
        if (steamCol.activeSelf == false)
        {
            hasSteam = false;
            trigger.active = false;
            anim.SetTrigger("turbineOff");
            isAnimPlaying = false;
        }

        AudioSource.mute = !hasSteam;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Steam")
        {
            hasSteam = true;
        }
    }
}
