using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationCycleOffset : MonoBehaviour
{
    public float AnimationCycleOffet;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        animator.SetFloat("AnimationCycleOffset", AnimationCycleOffet);
    }
}
