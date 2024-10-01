using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollisionEvent : MonoBehaviour
{
    public UnityEvent CollisionEnterEvent;
    public UnityEvent CollisionExitEvent;

    private void OnTriggerEnter(Collider other)
    {
        CollisionEnterEvent.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        CollisionExitEvent.Invoke();
    }
}
