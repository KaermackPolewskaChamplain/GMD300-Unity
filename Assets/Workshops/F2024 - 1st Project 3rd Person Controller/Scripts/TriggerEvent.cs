using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : MonoBehaviour
{
    public UnityEvent<Collider> TriggerEnterEvent;
    public UnityEvent TriggerExitEvent;

    private void OnTriggerEnter(Collider other)
    {
        TriggerEnterEvent.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        TriggerExitEvent.Invoke();
    }
}
