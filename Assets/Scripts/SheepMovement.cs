using UnityEngine;
using UnityEngine.AI;

public class SheepMovement : MonoBehaviour
{   
    public Transform player;

    [Header("Run Settings")]
    public float runDistance = 20f;
    public float panicDistance = 5f;

    [Header("Idle Settings")]
    public float idleDistance = 4f;
    public float idleTime = 5f;

    private NavMeshAgent navMeshAgent;
    private float idleTimer;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;

        // find current distance to player
        float currDistance = Vector3.Distance(transform.position, player.position);

        // if within distance, run
        if (currDistance < panicDistance)
        {
            // find the direction in lieu to player
            Vector3 directionToSheep = transform.position - player.position;
            // calc destination to run to (opposite to player)
            Vector3 newDestination = transform.position + directionToSheep.normalized * runDistance;
            // set destination
            navMeshAgent.SetDestination(newDestination);

            // set a timer
            idleTimer = idleTime;
        }
        else // want sheep to idle
        {
            
        }
        
    }
}
