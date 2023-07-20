using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCPatrol : MonoBehaviour
{
    public List<Transform> waypoints;
    public float patrolTime = 10f;
    public float speed = 2f;
    public float idleSpeed = 0f;
    public float rotationSpeed = 5f; // speed of rotation towards painting
    private NavMeshAgent agent;
    private Animator animator;
    private Transform currentDestination;
    private float patrolTimer;
    private bool isWaiting = false;
    private NPCspawning npcData;

    // Initialize the NavMeshAgent and Animator
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        patrolTimer = patrolTime;
        npcData = transform.parent.GetComponent<NPCspawning>();
        GetNewWaypoint();
    }

    private void Update()
    {
        patrolTimer += Time.deltaTime;

        // Walk to the destination
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            // If we reached the destination and the patrol time has elapsed
            if (patrolTimer >= patrolTime && !isWaiting)
            {
                StartCoroutine(WaitAtDestination(Random.Range(25f, 30f)));
            }
            else // Otherwise, rotate towards the painting
            {
                LookAtPainting();
            }
        }

        // Manage the animation
        if (agent.velocity.sqrMagnitude > 0f) // if the agent is moving
        {
            animator.SetFloat("Speed", speed);
        }
        else // if the agent is not moving
        {
            animator.SetFloat("Speed", idleSpeed);
        }
    }

    private void GetNewWaypoint()
    {
        if (npcData.waypoints.Count == 0)
            return;

        int randomIndex = Random.Range(0, npcData.waypoints.Count);
        currentDestination = npcData.waypoints[randomIndex];
        agent.SetDestination(currentDestination.position);
    }

    private void LookAtPainting()
    {
        // Check if the waypoint has a parent
        if (currentDestination.parent != null)
        {
            // Determine which direction to rotate towards
            Vector3 direction = (currentDestination.parent.position - transform.position).normalized;

            // Make the rotation only horizontal
            direction.y = 0f;

            // Create the rotation we need to be in to look at the target
            Quaternion lookRotation = Quaternion.LookRotation(direction);

            // Rotate us over time according to speed until we are in the required rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }

    private IEnumerator WaitAtDestination(float duration)
    {
        isWaiting = true;
        // int idleState = Random.Range(0, 4); // select a random idle animation
        // animator.SetInteger("IdleState", idleState);
        yield return new WaitForSeconds(duration);
        isWaiting = false;
        GetNewWaypoint();
        // animator.SetInteger("IdleState", 0); // reset idle animation state
        patrolTimer = 0f;
    }
    
}

