using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//Forces the Character Controller component onto the gameobject we attach this script
[RequireComponent(typeof(CharacterController))]
public class FirstPersonInputController : MonoBehaviour
{
    //Movement speed values
    public float BaseMovementSpeed = 2f; //In units per second
    public float SprintMovementSpeed = 5f; //In units per second
    public float MovementAccelerationTime = 0.1f; //In seconds

    public float MaxBaseRotation = 90f; //In euler degrees
    public float MaxSprintRotation = 135f; //In euler degrees
    public float RotationAccelerationTime = 0.1f; //In seconds
    public float MouseSensitivity = 5; //Check the ProcessInputRotation() method below for more info

    public float MaxJumpHeight = 1; //In units

    //Component & asset references
    public InputActionAsset CharacterInputActions;
    public Camera FirstPersonCamera;
    private CharacterController characterController;

    //Extracted Input Actions
    private InputAction moveAction;
    private InputAction sprintAction;
    private InputAction rotateAction;
    private InputAction jumpAction;

    //Current frame input values
    private Vector2 inputMovement = Vector2.zero;
    private Vector2 previousFrameInputMovement = Vector2.zero;
    private Vector2 inputRotation = Vector2.zero;
    private Vector2 previousFrameInputRotation = Vector2.zero;
    private float verticalMovement = 0;
    private float horizontalRotation = 0;
    private float verticalRotation = 0;

    //Input state tracking
    private bool isSprinting = false;
    private bool isJumping = false;
    
    //VERY IMPORTANT! Make sure to enable the ActionMap when this gets enabled. Otherwise you won't be able to capture inputs.
    //You can also Enable/Disable action maps while the player is in menus to limit character input when needed
    private void OnEnable()
    {
        //This enables the Gameplay Action Map
        CharacterInputActions.FindActionMap("Gameplay").Enable();
    }

    private void OnDisable()
    {
        //This disables the Gameplay Action map.
        CharacterInputActions.FindActionMap("Gameplay").Disable();
    }

    //It is good practice to use the Awake() method for Self-Initialization.
    //Since Awake is triggered before the Start method, you can be sure that everything is initialized properly by then.
    //The Start() method can then be used to read values from other components safely, knowing that they were all self-initialized properly.
    private void Awake()
    {
        SetupValidationChecks();

        SetupInputActions();

        SetupComponentReferences();

        SetupCursor();
    }

    private void SetupValidationChecks()
    {
        bool hasFailedValidation = false;

        if(CharacterInputActions == null)
        {
            Debug.LogError("Input Actions asset reference is missing! Please assign it to the First Person Input Controller component.", this);
            hasFailedValidation = true;
        }

        if (FirstPersonCamera == null)
        {
            Debug.LogError("Player Character Camera reference is missing! Please assign it to the First Person Input Controller component.", this);
            hasFailedValidation = true;
        }else if (FirstPersonCamera.transform.parent != this.transform)
        {
            Debug.LogError("Player Character Camera must be attached to the Player Character game object! Please parent the camera to the character.", this);
            hasFailedValidation = true;
        }

        if (hasFailedValidation)
        {
            gameObject.SetActive(false); // Disable the Player controller until errors are resolved.
        }
    }

    void SetupInputActions()
    {
        //Get the InputActions from the ActionMaps within the InputAction asset (That's a mouthfull!)
        //These names must match those in the InputAction Asset, otherwise these inputs won't work.
        moveAction = CharacterInputActions.FindActionMap("Gameplay").FindAction("Move");
        rotateAction = CharacterInputActions.FindActionMap("Gameplay").FindAction("Rotate");
        jumpAction = CharacterInputActions.FindActionMap("Gameplay").FindAction("Jump");
        sprintAction = CharacterInputActions.FindActionMap("Gameplay").FindAction("Sprint");
    }

    private void SetupComponentReferences() 
    {
        characterController = GetComponent<CharacterController>();
    }

    //This ensures the mouse doesn't move everywhere around the screen when we are playing
    private void SetupCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        ProcessSprintInput();
        ProcessRotationInput();
        ProcessMovementInput();
        ProcessVerticalMovementInput();
    }

    //Sprinting input is processed first as it affects movement and rotation speed
    private void ProcessSprintInput()
    {
        //Since sprinting is mapped onto a button, we can simply check IsPressed()
        isSprinting = sprintAction.IsPressed();
    }

    private void ProcessRotationInput()
    {
        //Read the rotate input vector for this frame
        inputRotation = rotateAction.ReadValue<Vector2>();

        //The inputRotation value can be actually above 1 when using the mouse.
        //This is due to the OS mouse acceleration settings partly, along Unity mouse input scaling as described below.
        //We clamp the maximum inputRotation with the set mouse sensitivity to ensure a good input feel.
        //This is generally a highly personal setting as it depends of your current mouse DPI and whether mouse acceleration is enabled or not on your computer
        inputRotation = Vector2.ClampMagnitude(inputRotation, MouseSensitivity);

        //AN IMPORTANT NOTE REGARDING MOUSE DELTA MOVEMENT SCALING
        //Make sure that the Mouse delta value in your InputActionAsset has a Scaling Processor set to (0.05, 0.05)
        //Unfortunate this magic number reflects the old input system in Unity and a more comfortable scaling with mouse movement
        //Read this if you like to delve into this technical debate! https://forum.unity.com/threads/mouse-delta-input.646606/#post-5044517
        
        if (isSprinting)
        {
            inputRotation *= MaxSprintRotation;
        }
        else
        {
            inputRotation *= MaxBaseRotation;
        }

        //Check the ProcessInputMovement() method below for more info on why we are lerping here!
        inputRotation = Vector2.Lerp(previousFrameInputRotation, inputRotation, Time.deltaTime / RotationAccelerationTime);
        previousFrameInputRotation = inputRotation;
    }

    private void ProcessMovementInput()
    {
        //Read the move input vector for this frame
        inputMovement = moveAction.ReadValue<Vector2>();

        //Does the player wants to sprint? If so, multiply the move input by the sprint speed, otheriwise multiply by the base speed
        if (isSprinting)
        {
            inputMovement *= SprintMovementSpeed;
        }
        else
        {
            inputMovement *= BaseMovementSpeed;
        }

        //This process below allows to accelerate / deccelerate movement within a specific time.
        //What we do here is taking the previous frame movement value and bringing it closer to the desired input movement.
        //Lerp is very useful to gradually go from value A to B within a certain time.
        //Lerp(A, B, Time.deltaTime / #seconds) = Value A gradually transforming into B over the set amount of time (#seconds)
        inputMovement = Vector2.Lerp(previousFrameInputMovement, inputMovement, Time.deltaTime / MovementAccelerationTime);

        //And finally we save the current frame input movement for next frame
        previousFrameInputMovement = inputMovement;
    }

    private void ProcessVerticalMovementInput()
    {
        if(characterController.isGrounded && verticalMovement < 0)
        {
            isJumping = false;
            verticalMovement = 0;
        }

        //This is the equivalent of checking GetButtonDown. It is true only the first frame the button is pressed, then it becomes false.
        //Highly useful for situation where you don't want to track the input the whole time the button is pressed
        bool jumpButtonDown = jumpAction.triggered && jumpAction.ReadValue<float>() > 0;

        Debug.Log(verticalMovement +" " +characterController.isGrounded + " " + jumpButtonDown);

        //If the jumpAction was triggered this frame, this will trigger the jumping sequence, otherwise it sets isJumping to false
        //This condition and the one above could both possibly happen the same frame. This would be a fine case, but a rare occurence
        if (jumpButtonDown && characterController.isGrounded)
        {
            isJumping = true;

            //Boost the vertical movement by countering the effect of gravity
            verticalMovement += Mathf.Sqrt(MaxJumpHeight * -2.0f * Physics.gravity.y);
        }

        //Apply gravity no matter what. The jumping strength will compensate against it when necessary.
        verticalMovement += Physics.gravity.y * Time.deltaTime;
    }

    private void LateUpdate()
    {
        //Late update always happens right after the update method is completed for the frame
        //Placing the character controller movement update here ensures that we get the final input and jump movement values for the frame, especially if we were to modify it through other objects
        //If we wanted to use physics to affect the character, we should move in the Fixed Update though, as all physic operations must be completed within a precise timing. That would imply using a rigidbody though.

        ApplyInputRotation();
        ApplyInputMovement();
    }

    private void ApplyInputRotation()
    {
        //Quaternions are used to represents rotation in Unity. They are complex to understand! But they also offer many benefits.
        //Because of their complexity, it is often better to calculate Euler angles (Degrees) first and then transfer them to Quaternions.
        //Quaternions can be added together using the multiplication sign
        //Don't forget how rotation works. The Y axis manages horizontal rotation! Imagine a skewer stucked to the Y axis of the object. When you turn it, the object will rotates horizontally.

        horizontalRotation += inputRotation.x * Time.deltaTime;
        verticalRotation -= inputRotation.y * Time.deltaTime;
        verticalRotation = Mathf.Clamp(verticalRotation, -85f, 85f);

        transform.rotation = Quaternion.Euler(0, horizontalRotation, 0);
        FirstPersonCamera.transform.rotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0);
    }

    private void ApplyInputMovement()
    {
        //TransformDirection will ensure that the movement is relative to the character. Z forward will always mean character forward. X sideways will be the character sides.
        Vector3 movementToApply = transform.TransformDirection(new Vector3(inputMovement.x, 0, inputMovement.y));

        //Add the vertical movement to the movement
        movementToApply += new Vector3(0, verticalMovement, 0);

        //Finally move the character controller with the combined movement
        characterController.Move(movementToApply * Time.deltaTime);
    }

//Compiler condition! We tell Unity to only compile that part of code when in the editor, but never in a build
//This gives us a nice visual debug view of the player character position in the editor
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        //If we're in the editor, and not playing the game, get the Character Controller component
        //This is required since the Awake method above won't be called in the particular situation
        //We need the Character Controller component reference to properly display the debug
        if (!Application.isPlaying && characterController == null)
        {
            characterController = GetComponent<CharacterController>();
        }

        //Half transparent cyan color. A vector 4 can be used as a color input. RGBA = XYZW
        Gizmos.color = new Vector4(0, 1, 1, 0.5f);

        //Calculate the height offset between the top and bottom of the characterController
        Vector3 debugSphereOffset = new Vector3(0, characterController.height / 2 - characterController.radius, 0);

        //Draws a debug sphere at the bottom and top of the character controller
        Gizmos.DrawSphere(transform.position + characterController.center - debugSphereOffset, characterController.radius);
        Gizmos.DrawSphere(transform.position + characterController.center + debugSphereOffset, characterController.radius);
    }
#endif
}