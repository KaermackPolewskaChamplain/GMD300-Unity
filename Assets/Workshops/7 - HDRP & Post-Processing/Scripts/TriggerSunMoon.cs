using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSunMoon : MonoBehaviour
{
    public Light Sun;
    public Light Moon;

    public bool ShowSun;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if (ShowSun)
            {
                Sun.gameObject.SetActive(true);
                Moon.gameObject.SetActive(false);
            }
            else
            {
                Sun.gameObject.SetActive(false);
                Moon.gameObject.SetActive(true);
            }
        }
    }
}
