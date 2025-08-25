using UnityEngine;
using UnityEngine.AI;

public class EnemyTankMovement : MonoBehaviour
{
    // The tank will stop moving towards the player once it reaches this distance
    public float m_CloseDistance = 8f;
    // The tank's turret object
    public Transform m_Turret;

    // A reference to the player - this will be set when the enemy is loaded
    private GameObject m_Player;
    // A reference to the NavMeshAgent component on the tank
    private NavMeshAgent m_NavAgent;
    // A reference to the Rigidbody component
    private Rigidbody m_Rigidbody;

    // Will be set to true when this tank should follow the player
    private bool m_Follow;

    private void Awake()
    {
        m_Player = GameObject.FindGameObjectWithTag("Player");
        m_NavAgent = GetComponent<NavMeshAgent>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Follow = true;
    }

    private void OnEnable()
    {
        m_Rigidbody.isKinematic = false;

        // Ensure the NavMeshAgent is placed correctly on the NavMesh
        if (m_NavAgent != null && m_NavAgent.isOnNavMesh == false)
        {
            // Warp the agent to its current position so it attaches to the NavMesh
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 5f, NavMesh.AllAreas))
            {
                m_NavAgent.Warp(hit.position);
            }
        }
    }

    private void OnDisable()
    {
        m_Rigidbody.isKinematic = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            m_Follow = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            m_Follow = false;
        }
    }

    void Update()
    {
        if (!m_Follow || m_Player == null || m_NavAgent == null)
            return;

        if (!m_NavAgent.isOnNavMesh)
            return; // Prevent errors when agent is not on NavMesh

        // Get distance from player to enemy tank
        float distance = Vector3.Distance(m_Player.transform.position, transform.position);

        // If distance is greater than stop distance, move towards player
        if (distance > m_CloseDistance)
        {
            if (!m_NavAgent.isStopped)
            {
                m_NavAgent.isStopped = false;
            }
            m_NavAgent.SetDestination(m_Player.transform.position);
        }
        else
        {
            if (!m_NavAgent.isStopped)
            {
                m_NavAgent.isStopped = true;
            }
        }

        // Rotate turret to face player
        if (m_Turret != null)
        {
            Vector3 targetPosition = m_Player.transform.position;
            targetPosition.y = m_Turret.position.y; // Keep rotation only on Y axis
            m_Turret.LookAt(targetPosition);
        }
    }
}
