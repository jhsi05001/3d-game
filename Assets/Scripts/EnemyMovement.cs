using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent navMeshAgent;
    
    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            navMeshAgent.SetDestination(player.position);
        }
    }
}
