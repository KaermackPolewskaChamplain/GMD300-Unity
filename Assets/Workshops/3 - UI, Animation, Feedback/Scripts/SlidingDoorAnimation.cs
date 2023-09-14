using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SlidingDoorAnimation : MonoBehaviour
{
    private Animator animator;

    public void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void OpenDoor()
    {
        animator.SetBool("isOpened", true);
    }

    public void CloseDoor()
    {
        animator.SetBool("isOpened", false);
    }
}
