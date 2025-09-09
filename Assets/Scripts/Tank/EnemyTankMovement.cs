using UnityEngine;
using UnityEngine.AI;

public class EnemyTankMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float m_CloseDistance = 8f;
    public Transform m_Turret;
    public Transform m_Player;

    private NavMeshAgent m_NavAgent;
    private Rigidbody m_Rigidbody;

    [Header("Random Movement Settings")]
    public float roamRadius = 20f; // How far enemy can roam
    public float roamInterval = 3f; // Change position every 3 seconds
    private float nextRoamTime;

    private bool isChasing = false;

    [Header("Turret Settings")]
    public float turretRotationSpeed = 3f;

    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 2f;
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

    void Start()
    {
        // Ignore collisions with player
        Collider playerCollider = GameObject.FindWithTag("Player").GetComponent<Collider>();
        Collider enemyCollider = GetComponent<Collider>();
        Physics.IgnoreCollision(enemyCollider, playerCollider);

        // Ensure agent is on NavMesh
        if (m_NavAgent != null && !m_NavAgent.isOnNavMesh)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 5f, NavMesh.AllAreas))
            {
                m_NavAgent.Warp(hit.position);
            }
        }

        nextRoamTime = Time.time + roamInterval;
    }

    void Update()
    {
        if (m_NavAgent == null || !m_NavAgent.isActiveAndEnabled || !m_NavAgent.isOnNavMesh) return;

        if (isChasing)
        {
            FollowPlayer();
            RotateTurret();
            TryShoot();
        }
        else
        {
            RandomRoamEvery3Sec();
        }
    }

    // =================== RANDOM ROAM EVERY 3 SECONDS ===================
    void RandomRoamEvery3Sec()
    {
        if (Time.time >= nextRoamTime)
        {
            Vector3 newDestination = GetRandomNavMeshPosition(transform.position, roamRadius);
            if (newDestination != Vector3.zero)
            {
                m_NavAgent.isStopped = false;
                m_NavAgent.SetDestination(newDestination);
            }
            nextRoamTime = Time.time + roamInterval;
        }
    }

    Vector3 GetRandomNavMeshPosition(Vector3 origin, float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += origin;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return Vector3.zero;
    }

    // =================== CHASE LOGIC ===================
    private void FollowPlayer()
    {
        float distance = Vector3.Distance(m_Player.position, transform.position);

        if (distance > m_CloseDistance)
        {
            if (m_NavAgent.isStopped)
                m_NavAgent.isStopped = false;

            if (m_NavAgent.isOnNavMesh)
                m_NavAgent.SetDestination(m_Player.position);
        }
        else
        {
            if (!m_NavAgent.isStopped)
                m_NavAgent.isStopped = true;
        }
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

            Destroy(bullet, 5f);
        }
    }

    // =================== TRIGGER DETECTION ===================
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isChasing = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isChasing = false;
            nextRoamTime = Time.time + roamInterval; // Reset timer after chase
        }
    }
}
