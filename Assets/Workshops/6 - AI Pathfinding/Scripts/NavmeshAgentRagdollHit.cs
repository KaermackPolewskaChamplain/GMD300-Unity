using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody), typeof(NavMeshAgent))]
public class NavmeshAgentRagdollHit : MonoBehaviour
{
    private NavMeshAgent agent;
    private Rigidbody rb;
    private Vector3 agentDestination;

    bool isRagdoll = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (isRagdoll)
        {
            //Check if the velocity is low enough and restart the Navmesh agent
            if(rb.velocity.magnitude < 0.1f)
            {
                //We first check if we are close to the navmesh, otherwise we may still be in the air
                //If so:
                // - We disable the rigidbody physic movement (isKinematic = true)
                // - We warp the agent position to the navMeshHit position (We have to do so, since otherwise the navmesh agent will teleport back to its previous position before the ragdoll
                // - We update the destination to ensure that the navmesh agent path is updated
                // - We set isRagdoll to false

                NavMeshHit navMeshHit;
                if(NavMesh.SamplePosition(rb.position, out navMeshHit, 0.5f, NavMesh.AllAreas))
                {
                    rb.isKinematic = true;

                    agent.Warp(navMeshHit.position);
                    agent.updatePosition = true;
                    agent.SetDestination(agentDestination);

                    isRagdoll = false;
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "NavMeshPhysicObstacle" && isRagdoll == false)
        {
            agentDestination = agent.destination;
            agent.ResetPath();
            agent.updatePosition = false;

            //Make the agent movable by physics
            rb.isKinematic = false;

            //You should normally use Add Force, but since we are using VelocityChange as the ForceMode anyway, this has the advantage of updating the velocity the first frame the force is applied
            //Otherwise, using AddForce, the first frame velocity magnitude is equal to 0, which leads to issue in the update loop above where we wanna check if the character is ragdoll and their velocity is set to 0, which would trigger before the character is set out to space :)
            //The velocity is hardcoded here for the sake of the example, but you shouldn't do that in your projects :)
            rb.velocity = collision.impulse.normalized * 20 + Vector3.up * 5;

            isRagdoll = true;
        }
    }
}
