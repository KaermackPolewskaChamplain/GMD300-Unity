using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//You must use DG.Tweening in order to use DOTween
using DG.Tweening;

[RequireComponent(typeof(Animator))]
public class SlidingDoorAnimation : MonoBehaviour
{
    private Animator animator;

    public float CameraShakeDuration = 0.4f;
    public float CameraShakeStrength = 0.1f;
    public int CameraShakeVibrato = 10;

    public float SlidingDoorLength = 2.0f;
    public Transform DustParticlesSpawnAnchor;
    public ParticleSystem DustParticlesPrefab;

    private void Awake()
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

    //Where is this called?
    //Directly in the SlidingDoorOpen/SlidingDoorClose Animation Clips!
    //You can add events to animation clips to call functions at specific times, similarly to Unity Events.
    //That means that when the animator plays that clip, an event calls this function at the specified frame
    public void PlaysCameraShake(float intensityScale)
    {
        //DOTween is easily accessible through shortcuts with many common components from Unity like Transform, Rigidbody and Material
        //Camera.main is also a shortcut to get the first active camera in the scene. It's fine to use when you only have 1 camera, but it can cause issues otherwise.
        Camera.main.transform.DOShakePosition(CameraShakeDuration, CameraShakeStrength * intensityScale, CameraShakeVibrato);
    }

    //Where is this called?
    //Directly in the SlidingDoorOpen/SlidingDoorClose Animation Clips!
    //You can add events to animation clips to call functions at specific times, similarly to Unity Events.
    //That means that when the animator plays that clip, an event calls this function at the specified frame
    public void PlayDustParticles()
    {
        //Make a copy instance of DustParticlesPrefab and attach it to the sliding door
        //(The part of the sliding door that doesn't move, or the particles will follow the door!)
        ParticleSystem particlesCopy = Instantiate(DustParticlesPrefab, DustParticlesSpawnAnchor);

        //Then we play the particle system!
        //NOTE: You must set the particle system to destroy itself after all particles are finished rendering,
        //or you'll end up with too many unused particles in the scene, which will lead to bad performance.
        particlesCopy.Play();
    }
}
