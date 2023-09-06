using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventRaycastSwitchPlate : MonoBehaviour
{
    //Here we use the UnityEvent OnSwitchButtonPress to register what will happen when the button is pressed
    //The outcome to the event must be set in the inspector. You can reference any object from the scene and
    //call any script and function that you'd like to be triggered when the button is pressed
    public UnityEvent OnSwitchButtonPress;

    //OnInteract will be called by the RaycastInteractionSystem using SendMessage
    public void OnInteract()
    {
        OnSwitchButtonPress.Invoke();
    }
}
