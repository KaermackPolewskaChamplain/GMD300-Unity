using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(PlayableDirector))]
public class CinematicTimelineTrigger : MonoBehaviour
{
    public bool canPlay = true;

    private PlayableDirector timelineDirector;
    private ThirdPersonControllerWithCinemachine playerReference;

    private void Awake()
    {
        timelineDirector = GetComponent<PlayableDirector>();

        //See https://docs.unity3d.com/ScriptReference/Playables.PlayableDirector-stopped.html
        //We are attaching an event, similarly to Unity Events, but through code!
        //This triggers when the timeline stops playing.
        //That's better for performance than checking the playableDirector state in the update loop!
        timelineDirector.stopped += OnTimelineStopped;
    }

    void OnTimelineStopped(PlayableDirector somePlayableDirector)
    {
        //We first check if it's really our cinematic that ended, and not some other timeline.
        if(timelineDirector == somePlayableDirector)
        {
            playerReference.canControl = true;
        }
    }

    //When colliding with the trigger box, we check if we hit the player.
    //If so, we save the player reference for later.
    //We set canControl to false on the player so that the character can't be moved.
    //We start the timeline cinematic.
    //We set canPlay to false, so that we can't replay that cinematic anymore.
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" && canPlay)
        {
            playerReference = other.GetComponent<ThirdPersonControllerWithCinemachine>();

            timelineDirector.Play();

            playerReference.canControl = false;

            canPlay = false;
        }
    }
}
