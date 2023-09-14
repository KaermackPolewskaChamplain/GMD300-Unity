using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RaycastInteractionSystem : MonoBehaviour
{
    public float MaxInteractionDistance;

    public InputActionAsset CharacterInputActions;

    public Camera FirstPersonCamera;

    private InputAction interactAction;

    private void Awake()
    {
        //We grab the interact input from the action map
        interactAction = CharacterInputActions.FindActionMap("Gameplay").FindAction("Interact");
    }

    private void Update()
    {
        RaycastInteraction();
    }

    private void RaycastInteraction()
    {
        //This boolean is used to track whether or not we found an interactible, to trigger the UI interactible panel
        bool foundInteractible = false;

        //Let's build a laser..! Starting at the camera position and pointing forward. This will be the base of the raycast.
        Ray ray = new Ray(FirstPersonCamera.transform.position, FirstPersonCamera.transform.forward);

        //HitInfo will be used to keep the Raycast Hit information once we raycast
        RaycastHit hitInfo;

        //Here is where the magic happens. We will ask the Physic engine of unity to cast the ray we built above up to the MaxInteractionDistance
        //It will catch any collision that happens into hitinfo
        //Physics raycast returns True or False whether or not we got a hit using the ray and distance, so we only check the remainder of the code if it hits something
        if(Physics.Raycast(ray, out hitInfo, MaxInteractionDistance))
        {
            //We'll check the object tag. If it is Interactible, we'll send a Unity Message to all components
            //This is NOT optimal at all, but it is a simple example to showcase the abilities
            //A better system would use an interface and a layer filtering system for instance, or an event system
            if (hitInfo.transform.tag == "Interactible")
            {
                foundInteractible = true;

                //We check if the interact button is down for this frame only
                bool interactButtonDown = interactAction.triggered && interactAction.ReadValue<float>() > 0;

                //If the button is pressed...
                if (interactButtonDown)
                {
                    //We send a message to the hit object to start the method OnInteract of any component
                    //We set SendMessageOptions.DontRequireReceiver to make sure Unity doesn't throw an error if there is no receiving function
                    hitInfo.transform.SendMessage("OnInteract", SendMessageOptions.DontRequireReceiver);
                }
            }
        }

        //We send the foundInteractible boolean value to the UI Manager, to control the visibility of the interact prompt
        UIManager.Instance.ShowInteractPrompt(foundInteractible);
    }
}
