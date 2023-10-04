using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavmeshAgentPatrol : MonoBehaviour
{
    //Directly taken from here: https://docs.unity3d.com/Manual/nav-AgentPatrol.html
    //...but slightly modified.

    public Transform[] PatrolPoints;

    private int currentDestinationPoint;
    private NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        // Disabling auto-braking allows for continuous movement
        // between points (ie, the agent doesn't slow down as it
        // approaches a destination point).
        agent.autoBraking = false;

        GotoNextPoint();
    }

    private void GotoNextPoint()
    {
        // Returns if no points have been set up
        if (PatrolPoints.Length == 0)
            return;

        // Set the agent to go to the currently selected destination.
        agent.destination = PatrolPoints[currentDestinationPoint].position;

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        // % sign is the Modulo operator. It gives us the remainder of the division.
        // It allows to loop the array easily.
        // For instance: 0%2=0, 1%2=1, 2%2=0, 3%2=1, 4%2=0, 5%2=1, etc.
        currentDestinationPoint = (currentDestinationPoint + 1) % PatrolPoints.Length;
    }

    private void Update()
    {
        // Choose the next destination point when the agent gets
        // close to the current one.
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            GotoNextPoint();
        }
    }
}
