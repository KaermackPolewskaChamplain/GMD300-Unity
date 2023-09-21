using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class ThirdPersonController : MonoBehaviour
{
    private CharacterController characterController;
    private PlayerInput playerInput;

    public float moveMaxSpeed = 6.0f;
    public float moveTimeToMaxSpeed = 0.5f;

    public float sprintSpeedMultiplier = 2f;
    public float sprintAccelerationMultiplier = 0.5f;

    //Affects the current velocity in air
    public float airSpeedMultiplier = 0.5f;
    //Affects how acceleration is applied in air
    public float airAccelerationMultiplier = 0.5f;

    public float jumpAcceleration = 2;
    public float jumpMaxTime = 0.25f;

    public float coyoteJumpMaxTime = 0.1f;

    public LayerMask interactibleLayer;
    public Transform interactionRangeAnchor;
    public Vector3 interactionRangeSize;

    public Transform attackRangeAnchor;
    public Vector3 attackRangeSize;

    private Vector2 moveInput = Vector2.zero;

    private Vector2 currentHorizontalVelocity = Vector2.zero;
    private float currentVerticalVelocity = 0;

    private float targetSpeed = 0;
    private float targetAcceleration = 0;

    private float lastTimeGrounded = 0;
    private float lastTimeJumpInputPressed = 0;
    private float jumpTimer = 0;

    private bool isJumping = false;
    private bool isSprinting = false;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();

        //Should be set to 0 as stipulated by Unity's documentation
        characterController.minMoveDistance = 0;
    }

    private void Update()
    {
        CalculateVerticalMovement();
        CalculateHorizontalMovement();

        RotateCharacter();
        MoveCharacter();
    }

    private void CalculateVerticalMovement()
    {
        //We check if we can actually jump first
        //If so, set the states and start the jump timer
        if (JumpCheck())
        {
            isJumping = true;
            jumpTimer = 0;

            currentVerticalVelocity = jumpAcceleration;
        }

        if (isJumping)
        {
            jumpTimer += Time.deltaTime;

            if(jumpTimer >= jumpMaxTime)
            {
                isJumping = false;
            }
        }
        else
        {
            float gravityFrameStep = Physics.gravity.y * Time.deltaTime;

            currentVerticalVelocity += gravityFrameStep;

            //Make sure to clamp the minimum vertical movement when grounded, otherwise gravity will accumulate too much between frames!
            //The vertical velocity cannot be 0 with a characterController, this is a quirk of the component...
            //Therefore we apply a small gravity force, equal to 1 frame worth, which forces ground collision
            if (characterController.isGrounded && currentVerticalVelocity < 0)
            {
                currentVerticalVelocity = gravityFrameStep;
            }
        }
    }

    //We want to be like Wile E. Coyote chasing Roadrunner through the air!
    //The CoyoteTime value needs to be small, ideally below 200ms.
    //The idea behind this is that we allow the player to jump within a few milliseconds of getting in the air or back on the ground.
    //That allows the player to jump during that time, since jumping is restricted to when the character is grounded
    //It is a very common feature of good platformer games.
    //Humans are actually quite slow to react.
    //A player could easily think they pressed the jump button at the right time on the edge of a platform, while in fact they were already in the air!
    //Or that same player could have pressed the jump button just before touching the ground. That feature allows the character to react as expected by the player
    //This helps make the game less frustrating, more accessible and not necessarily too easy if done well. It should not feel perceptible.
    private bool JumpCheck()
    {
        //If we're grounded, reset the isJumping flag
        if (characterController.isGrounded)
        {
            isJumping = false;
            lastTimeGrounded = Time.time; //Time.time is equal to the current time in seconds since the game started
        }

        //If lastTimeJumpInputPressed is equal to 0, that means it was never pressed or reset, therefore we skip the execution of this method
        //Otherwise we would jump on the first frame of the game always, since both time will be equal to 0!
        if (lastTimeJumpInputPressed == 0)
        {
            return false;
        }

        //We check the absolute time difference between the last time we were grounded and the last time we pressed the jump button
        float timeDifference = Mathf.Abs(lastTimeGrounded - lastTimeJumpInputPressed);

        //If the time difference is small enough, that means we can still jump!
        //...but only if we are not currently jumping.
        if (timeDifference <= coyoteJumpMaxTime)
        {
            if(timeDifference != 0)
            {
                Debug.Log("Coyote jump!");
            }

            lastTimeJumpInputPressed = 0; //Reset the value

            return true;
        }
        else
        {
            return false;
        }
    }

    private void CalculateHorizontalMovement()
    {
        targetSpeed = moveMaxSpeed;

        //This equation gives us the exact acceleration per second.
        //I personally prefer exposing the Max Speed and Time to Max Speed as values, as it is often more useful when balancing character movement
        //As a game designer, I want to fine tune what will be the max speed of the character and potentially the time to reach max speed
        //The acceleration per second is not as useful as those 2, therefore we use them to compute the acceleration
        targetAcceleration = moveMaxSpeed / moveTimeToMaxSpeed;

        //If I'm sprinting or in the air, modify the target speed and acceleration as needed
        if (isSprinting)
        {
            targetSpeed *= Mathf.Lerp(1, sprintSpeedMultiplier, moveInput.magnitude);
            targetAcceleration *= Mathf.Lerp(1, sprintAccelerationMultiplier, moveInput.magnitude);

            //Why use the magnitude here?
            //The player could be pressing the sprint button while they aren't moving.
            //That means they would get influenced by sprinting multipliers despite actually not moving
            //The magnitude provides us with the length of the moveInput vector.
            //When pushing fully in a direction, the magnitude will be equal to 1
            //When releasing the stick, the magnitude will be equal to 0
            //If the magnitude is equal to 0, the lerp value is 1, otherwise it's the sprint multiplier
        }

        if (!characterController.isGrounded)
        {
            targetSpeed *= airSpeedMultiplier;
            targetAcceleration *= airAccelerationMultiplier;
        }

        //To get the desired velocity, we use Lerp!
        //Vector2.Lerp(A, B, t) will give us the result of "A" gradually interpolating towards "B" at the speed "t"
        //If t = 0, the result of this will be A.
        //If t = 1, the result will be B.
        //If t = 0.5, the result will be A*0.5 + B*0.5
        //If t = 0.7, the result will be A*0.3 + B*0.7
        //The formula is therefore A*(1-t) + B*t

        //In our case:
        //A = currentHorizontalVelocity, which currently stores the velocity of the previous frame
        //B = moveInput * targetSpeed, which represents the maximum ideal speed in the direction we are pointing on the keyboard/controller
        //t = Time.deltaTime * targetAcceleration, which represent the acceleration we should take this frame
        //On its own, Time.deltaTime would take 1 full second to interpolate (read accelerate) fully towards B
        //By multiplying Time.deltaTime by targetAcceleration, we get the proper acceleration time

        currentHorizontalVelocity = Vector2.Lerp(currentHorizontalVelocity, moveInput * targetSpeed, Time.deltaTime * targetAcceleration);
    }

    private void RotateCharacter()
    {
        Vector3 lookDirection = new Vector3(currentHorizontalVelocity.x, 0, currentHorizontalVelocity.y).normalized;

        //We only rotate if the velocity is above 0.
        //Due to the nature of floating points, it's hard to have a velocity of exactly 0
        //Instead, we're taking a very small value
        if(lookDirection.magnitude > 0.001f)
        {
            Quaternion newRotation = Quaternion.LookRotation(lookDirection);

            //For rotation, we use Slerp instead of Lerp (Spherical Linear IntERPolation)
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * targetAcceleration);
        }
    }

    private void MoveCharacter()
    {
        Vector3 currentVelocity = new Vector3(currentHorizontalVelocity.x, currentVerticalVelocity, currentHorizontalVelocity.y);

        characterController.Move(currentVelocity * Time.deltaTime);
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();

        //Adjust the movement vector so that we smooth diagonal movement.
        //If we were to have moveInput = (1, 1), than the character would actually accelerate more than intended
        moveInput = moveInput.normalized * moveInput.magnitude;
    }

    public void OnSprint(InputValue value)
    {
        isSprinting = value.Get<float>() > 0;
    }

    public void OnJump(InputValue value)
    {
        bool jumpInputPressed = value.Get<float>() > 0;

        if (jumpInputPressed)
        {
            lastTimeJumpInputPressed = Time.time;
        }

        if (jumpInputPressed == false)
        {
            isJumping = false;
        }
    }

    public void OnInteract(InputValue value)
    {
        if (characterController.isGrounded)
        {
            OverlapInteractibleCheck();
        }
    }

    private void OverlapInteractibleCheck()
    {
        //Overlap box works similarly to a raycast. It provides us with a list of objects that were touched.
        //Instead of using a laser though, it uses a box and checks what object is inside of it.
        //Here we use interactibleLayer to only check overlaps with objects within the Interactible Layer
        //This improves performances and allows selecting only useful objects.
        Collider[] overlapInteractibles =   Physics.OverlapBox(
                                                interactionRangeAnchor.transform.position,
                                                interactionRangeSize / 2,
                                                interactionRangeAnchor.transform.rotation,
                                                interactibleLayer, 
                                                QueryTriggerInteraction.Collide);

        //If we actually got overlapped with interactible objects, the length of the array will be above 0
        if(overlapInteractibles.Length > 0)
        {
            //Let's check which is the closest
            //Since we have to compare between all touched interactibles, we'll set it to the first item for now
            Collider closestInteractible = overlapInteractibles[0];

            //If there is more than 1 overlapping interactible object, we need to choose which one to interact with
            //Here, the rule I've chosen is that only the closest interactible will be selected
            if (overlapInteractibles.Length > 1)
            {
                //Let's parse through all the interactibles
                foreach (Collider currentInteractible in overlapInteractibles)
                {
                    //Calculate the distance between player and the interactible currently set as closestInteractible
                    float distanceToClosestInteractible = Vector3.Distance(transform.position, closestInteractible.transform.position);
                    //Calculate the distance between player and the current interactible 
                    float distanceToCurrentInteractible = Vector3.Distance(transform.position, currentInteractible.transform.position);

                    //If the distance with currentInteractible is lower than the distance with closestInteractible,
                    //currentInteractible becomes the new closestInteractible and we continue parsing the lists
                    if (distanceToCurrentInteractible < distanceToClosestInteractible)
                    {
                        closestInteractible = currentInteractible;
                    }
                }
            }

            closestInteractible.SendMessage("OnPlayerInteract", SendMessageOptions.DontRequireReceiver);
        }
    }

    public void OnAttack(InputValue value)
    {
        OverlapAttackCheck();
    }

    private void OverlapAttackCheck()
    {
        Collider[] overlapHits =    Physics.OverlapBox(
                                        attackRangeAnchor.transform.position,
                                        attackRangeSize / 2,
                                        attackRangeAnchor.transform.rotation,
                                        Physics.AllLayers,
                                        QueryTriggerInteraction.Ignore);
            
        if(overlapHits.Length > 0)
        {
            foreach (Collider hit in overlapHits)
            {
                Vector3 hitDirection = hit.transform.position - transform.position;
                hit.SendMessage("OnPlayerAttack", hitDirection, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if(interactionRangeAnchor != null)
        {
            Gizmos.color = new Vector4(0, 1, 0, 0.5f);
            Gizmos.DrawCube(interactionRangeAnchor.transform.position, interactionRangeSize);
        }

        if(attackRangeAnchor != null)
        {
            Gizmos.color = new Vector4(1, 0, 0, 0.5f);
            Gizmos.DrawCube(attackRangeAnchor.transform.position, attackRangeSize);
        }
    }
}