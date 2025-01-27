using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    public float MaxMovementSpeed = 5.0f;
    public float MovementAcceleration = 10.0f;
    public float JumpForce = 1.0f;
    public float RotationAcceleration = 5.0f;
    public float ExternalForceMultiplier = 0.5f;

    CharacterController controller;

    bool jumpInput = false;

    Vector2 absoluteMoveInput = Vector2.zero;
    Vector3 relativeMoveInput = Vector3.zero;

    Vector3 maximumHorizontalVelocity = Vector3.zero;
    Vector3 currentHorizontalVelocity = Vector3.zero;

    Vector3 currentVerticalVelocity = Vector3.zero;

    Vector3 currentVelocity = Vector3.zero;

    Vector3 externalVelocity = Vector3.zero;

    void Awake()
    {
        Initialize();
    }

    /// <summary>
    /// Initializes the character movement components
    /// </summary>
    void Initialize()
    {
        controller = GetComponent<CharacterController>();
        controller.minMoveDistance = 0;
    }

    // Update is called once per frame
    void Update()
    {
        ProcessHorizontalVelocity();
        ProcessVerticalVelocity();

        RotateCharacter();

        MoveCharacter();
    }

    private void ProcessHorizontalVelocity()
    {
        //Get a an input direction relative to the camera forward direction
        relativeMoveInput = Camera.main.transform.TransformDirection(new Vector3(absoluteMoveInput.x, 0, absoluteMoveInput.y));
        relativeMoveInput.y = 0;
        relativeMoveInput = relativeMoveInput.normalized;

        //Compute the desired speed
        maximumHorizontalVelocity = relativeMoveInput;
        maximumHorizontalVelocity *= MaxMovementSpeed;

        //Process acceleration
        currentHorizontalVelocity = Vector3.Lerp(currentHorizontalVelocity, maximumHorizontalVelocity, MovementAcceleration * Time.deltaTime);
    }

    private void ProcessVerticalVelocity()
    {
        //Make sure to reset the gravity force when touching the ground
        if (controller.isGrounded)
        {
            currentVerticalVelocity = Vector3.zero;
        }

        //Check if the player can jump. If so, increase the vertical velocity by the value of JumpForce
        if (controller.isGrounded && jumpInput)
        {
            //A better way to calculate Jump force. Basically a jump force of 1 will allow you to reach up to 1 meter height
            float counterGravityJumpForce = Mathf.Sqrt(JumpForce * -2.0f * Physics.gravity.y);
            currentVerticalVelocity = new Vector3(0, counterGravityJumpForce, 0);
        }

        //Add the gravity to the vertical force
        currentVerticalVelocity += Physics.gravity * Time.deltaTime;

        //Clamp the downward velocity so that the character doesn't accelerate downwards for more than the gravity force
        currentVerticalVelocity = Vector3.Max(currentVerticalVelocity, Physics.gravity);
    }

    void MoveCharacter()
    {
        currentVelocity = currentHorizontalVelocity + currentVerticalVelocity;

        //Add the external velocity to the current velocity. This could come from
        //rigidbody that are touched by the Character Controller for instance
        currentVelocity += externalVelocity * ExternalForceMultiplier;

        controller.Move(currentVelocity * Time.deltaTime);
    }

    void RotateCharacter()
    {
        //LookRotation gives us the rotation angle of a vector direction. In this case, the character will face the direction of the relative move input
        //WARNING: Modifying the rotation of the character controller container like we are doing right now CAN conflict with the Cinemachine Virtual Camera Orbital Transposer
        //If you're having CrAzY rotation issues, make sure that the "Binding" setting in the Body Orbital Transposer is set to "World Space"
        if(relativeMoveInput.magnitude > 0)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(relativeMoveInput), Time.deltaTime * RotationAcceleration);
        }
    }

    void OnMove(InputValue input)
    {
        absoluteMoveInput = input.Get<Vector2>().normalized;
    }

    void OnJump(InputValue input)
    {
        jumpInput = (input.Get<float>() > 0) ? true : false;
    }

    public void AddExternalVelocity(Vector3 velocity)
    {
        externalVelocity = velocity;
    }
}