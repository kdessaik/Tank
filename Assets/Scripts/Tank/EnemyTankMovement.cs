using UnityEngine;
using UnityEngine.AI;

public class EnemyTankMovement : MonoBehaviour
{
    public float m_CloseDistance = 8f;         // Distance to stop near player
    public Transform m_Turret;                // Turret reference
    public Transform m_Player;                // Assign player in Inspector or find by tag

    private NavMeshAgent m_NavAgent;
    private Rigidbody m_Rigidbody;

    public Transform[] patrolPoints;          // Patrol points for idle movement
    private int currentPatrolIndex = 0;
    private bool isBlocked = false;

    private void Awake()
    {
        if (m_Player == null)
            m_Player = GameObject.FindGameObjectWithTag("Player").transform;

        m_NavAgent = GetComponent<NavMeshAgent>();
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        m_Rigidbody.isKinematic = false;

        if (m_NavAgent != null && !m_NavAgent.isOnNavMesh)
        {
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

    void Update()
    {
        if (m_Player == null || m_NavAgent == null || !m_NavAgent.isOnNavMesh)
            return;

        // Always follow the player
        FollowPlayer();

        // If blocked, try alternate path
        if (isBlocked)
        {
            ChooseRandomPatrolPoint();
        }

        // Rotate turret to face player
        if (m_Turret != null)
        {
            Vector3 targetPosition = m_Player.position;
            targetPosition.y = m_Turret.position.y;
            m_Turret.LookAt(targetPosition);
        }
    }

    private void FollowPlayer()
    {
        float distance = Vector3.Distance(m_Player.position, transform.position);

        if (distance > m_CloseDistance)
        {
            if (m_NavAgent.isStopped)
                m_NavAgent.isStopped = false;

            m_NavAgent.SetDestination(m_Player.position);
        }
        else
        {
            if (!m_NavAgent.isStopped)
                m_NavAgent.isStopped = true;
        }
    }

    private void ChooseRandomPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;

        currentPatrolIndex = Random.Range(0, patrolPoints.Length);
        m_NavAgent.SetDestination(patrolPoints[currentPatrolIndex].position);
        isBlocked = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Game Over");

            // OPTIONAL: Stop the enemy
            if (m_NavAgent != null)
                m_NavAgent.isStopped = true;

            // Exit game
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stops Play Mode in Editor
#else
            Application.Quit(); // Quits the game in a build
#endif
        }
    }
}
