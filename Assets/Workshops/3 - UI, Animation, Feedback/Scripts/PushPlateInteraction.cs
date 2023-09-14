using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class PushPlateInteraction : MonoBehaviour
{
    public UnityEvent OnPushPlatePress;
    public UnityEvent OnPushPlateRelease;

    private Animator animator;

    private void Awake()
    {
        //Since we are using simple animations here, we are using the legacy animation system, not Unity's Animator.
        animator = GetComponent<Animator>();

        animator.SetBool("isPressed", false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            OnPushPlatePress.Invoke();

            animator.SetBool("isPressed", true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            OnPushPlateRelease.Invoke();

            animator.SetBool("isPressed", false);
        }
    }
}
