using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerHitInteractions : MonoBehaviour
{
    //Push power
    public Vector3 pushForce = Vector3.one;

    private CharacterMovement characterMovement;

    private void Awake()
    {
        characterMovement = GetComponent<CharacterMovement>();
    }

    //Adapted from: https://docs.unity3d.com/ScriptReference/CharacterController.OnControllerColliderHit.html
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;

        // Check if the object we collided with is a rigidbody and
        // that it isn't kinematic. Otherwise, return.
        if (body == null || body.isKinematic)
        {
            return;
        }

        // Apply the velocity of the rigidbody onto our character controller
        //characterMovement.AddExternalVelocity(body.velocity);

        // Apply the push force to the rigidbody we collided with by
        // multiplying the direction of the Character Controller and the Push Force 
        body.velocity = Vector3.Scale(hit.moveDirection, pushForce);
    }
}
