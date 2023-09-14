using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideLavaInside : MonoBehaviour
{
    private bool isHidden = false;
    public void HideLava()
    {
        if (isHidden)
        {
            isHidden = false;
            gameObject.SetActive(true);
        }
        else
        {
            isHidden = true;
            gameObject.SetActive(false);
        }
    }
}
