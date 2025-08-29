using UnityEngine;
using UnityEngine.AI;

public class EnemyTankMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float m_CloseDistance = 8f; // Distance to stop near player
    public Transform m_Turret; // Turret reference
    public Transform m_Player; // Assign player in Inspector or via TankHealth script

    private NavMeshAgent m_NavAgent;
    private Rigidbody m_Rigidbody;

    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    private int currentPatrolIndex = 0;
    private bool isBlocked = false;

    [Header("Turret Settings")]
    public float turretRotationSpeed = 3f;

    [Header("Shooting Settings")]
    public GameObject bulletPrefab; // Assign bullet prefab in Inspector
    public Transform firePoint; // Assign the position where the bullet will spawn
    public float fireRate = 2f; // One shot every 2 seconds
    public float bulletSpeed = 20f;
    public float shootRange = 30f;

    private float nextFireTime;

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

        // If blocked, choose another destination
        if (isBlocked)
        {
            ChooseRandomPatrolPoint();
        }

        // Rotate turret smoothly to face player
        RotateTurret();

        // Check shooting condition
        TryShoot();
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
    void Start()
    {
        Collider playerCollider = GameObject.FindWithTag("Player").GetComponent<Collider>();
        Collider enemyCollider = GetComponent<Collider>();

        Physics.IgnoreCollision(enemyCollider, playerCollider);
    }


    private void RotateTurret()
    {
        if (m_Turret != null)
        {
            Vector3 targetPosition = m_Player.position;
            targetPosition.y = m_Turret.position.y;

            Vector3 direction = (targetPosition - m_Turret.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            m_Turret.rotation = Quaternion.Slerp(m_Turret.rotation, lookRotation, Time.deltaTime * turretRotationSpeed);
        }
    }

    private void TryShoot()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, m_Player.position);

        if (distanceToPlayer <= shootRange && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Shoot();
        }
    }

    private void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = firePoint.forward * bulletSpeed;
            }

            Destroy(bullet, 5f); // Destroy bullet after 5 seconds
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
    { }
}
