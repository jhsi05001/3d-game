using UnityEngine;
using UnityEngine.AI;

public class SheepMovement : MonoBehaviour
{   
    public Transform player;

    [Header("Run Settings")]
    public float runDistance = 20f; // distance sheep will run away
    public float fleeRange = 5f; // range at which sheep will start running away
    public float sheepRunSpeed = 6f; // speed at which sheep runs away

    [Header("Idle Settings")]
    public float idleDistance = 2f; // distante at which sheep idles from spot
    public float idleTime = 2f;
    public float sheepIdleSpeed = 3f; // speed at which sheep moves when idle

    private NavMeshAgent navMeshAgent;
    private float idleTimer = 0;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        idleTimer = idleTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;

        // find current distance to player
        float currDistance = Vector3.Distance(transform.position, player.position);

        // if within FLEE distance, run
        if (currDistance < fleeRange)
        {
            // find the direction in lieu to player
            Vector3 directionToSheep = transform.position - player.position;
            // calc destination to run to (opposite to player)
            Vector3 newDestination = transform.position + directionToSheep.normalized * runDistance;
            // set destination
            navMeshAgent.SetDestination(newDestination);
            navMeshAgent.speed = sheepRunSpeed;
        }
        // if within PANIC distance, run faster 

        // if safe, IDLE
        else // want sheep to idle
        {
            // set speed
            navMeshAgent.speed = sheepIdleSpeed;

            idleTimer += Time.deltaTime;

            if (idleTimer >= 5f && !navMeshAgent.hasPath && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                idleTimer = 0; // reset idle timer

                // set IDLE destination
                Vector3 randDirection = Random.insideUnitSphere * idleDistance;
                Vector3 newDestination = transform.position + randDirection;
                
                // check if newDestination is on the NavMesh
                NavMeshHit hit;
                if (NavMesh.SamplePosition(newDestination, out hit, idleDistance, NavMesh.AllAreas))
                {
                    newDestination = hit.position;
                }
                else
                {
                    newDestination = transform.position;
                }

                // set destination
                navMeshAgent.SetDestination(newDestination);
            }
        }
        
    }
}
