// ============================================================
// Script written by Yik Malehek
// Controls enemy tank movement, turret aiming, and shooting.
// Uses Unity NavMesh for movement and physics for bullets.
// ============================================================

using UnityEngine;
using UnityEngine.AI;

public class EnemyTankMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float m_CloseDistance = 8f; // Distance at which the tank stops near the player
    public Transform m_Turret; // Reference to the turret part of the enemy tank
    public Transform m_Player; // Reference to the player; can be assigned via Inspector or automatically in Awake()

    private NavMeshAgent m_NavAgent; // NavMesh agent to handle pathfinding
    private Rigidbody m_Rigidbody;   // Rigidbody for physics interactions

    [Header("Patrol Settings")]
    public Transform[] patrolPoints; // Optional patrol points when not chasing player
    private int currentPatrolIndex = 0; // Tracks which patrol point we�re heading to
    private bool isBlocked = false; // Flag used to choose a new patrol point if blocked

    [Header("Turret Settings")]
    public float turretRotationSpeed = 3f; // Speed at which the turret rotates towards the player

    [Header("Shooting Settings")]
    public GameObject bulletPrefab; // Prefab of the bullet to shoot
    public Transform firePoint;     // Transform from which bullets spawn
    public float fireRate = 2f;     // Time interval between shots (seconds)
    public float bulletSpeed = 20f; // Speed at which bullets travel
    public float shootRange = 30f;  // Max distance to shoot at the player

    private float nextFireTime; // Internal timer to handle fire rate

    // Called when the script instance is loaded
    private void Awake()
    {
        // Automatically find the player transform if not assigned in Inspector
        if (m_Player == null)
            m_Player = GameObject.FindGameObjectWithTag("Player").transform;

        // Cache references
        m_NavAgent = GetComponent<NavMeshAgent>();
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    // Called when the object is enabled
    private void OnEnable()
    {
        // Allow physics interactions
        m_Rigidbody.isKinematic = false;

        // Ensure NavMeshAgent is placed on a valid NavMesh location
        if (m_NavAgent != null && !m_NavAgent.isOnNavMesh)
        {
            NavMeshHit hit;
            // Sample nearby position to warp agent to a valid NavMesh
            if (NavMesh.SamplePosition(transform.position, out hit, 5f, NavMesh.AllAreas))
            {
                m_NavAgent.Warp(hit.position);
            }
        }
    }

    // Called when the object is disabled
    private void OnDisable()
    {
        // Freeze physics interactions when disabled
        m_Rigidbody.isKinematic = true;
    }

    // Called once per frame
    void Update()
    {
        // Early-out if missing references or off NavMesh
        if (m_Player == null || m_NavAgent == null || !m_NavAgent.isOnNavMesh)
            return;

        // Follow the player by default
        FollowPlayer();

        // If flagged as blocked, choose a random patrol point
        if (isBlocked)
        {
            ChooseRandomPatrolPoint();
        }

        // Smoothly rotate the turret to face the player
        RotateTurret();

        // Try to shoot at the player when conditions are met
        TryShoot();
    }

    // Handle following the player until within m_CloseDistance
    private void FollowPlayer()
    {
        float distance = Vector3.Distance(m_Player.position, transform.position);

        if (distance > m_CloseDistance)
        {
            // Resume moving if stopped
            if (m_NavAgent.isStopped)
                m_NavAgent.isStopped = false;

            // Update the NavMeshAgent�s destination to player�s position
            m_NavAgent.SetDestination(m_Player.position);
        }
        else
        {
            // Stop moving when close enough to the player
            if (!m_NavAgent.isStopped)
                m_NavAgent.isStopped = true;
        }
    }

    // Called before the first frame update
    void Start()
    {
        // Prevent collisions between this enemy and the player
        Collider playerCollider = GameObject.FindWithTag("Player").GetComponent<Collider>();
        Collider enemyCollider = GetComponent<Collider>();
        Physics.IgnoreCollision(enemyCollider, playerCollider);
    }

    // Rotate the turret smoothly to face the player�s position
    private void RotateTurret()
    {
        if (m_Turret != null)
        {
            // Target player�s position but keep turret�s current height
            Vector3 targetPosition = m_Player.position;
            targetPosition.y = m_Turret.position.y;

            // Compute direction and desired rotation
            Vector3 direction = (targetPosition - m_Turret.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);

            // Smoothly rotate turret using Slerp
            m_Turret.rotation = Quaternion.Slerp(m_Turret.rotation, lookRotation, Time.deltaTime * turretRotationSpeed);
        }
    }

    // Check if it�s time and within range to shoot at the player
    private void TryShoot()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, m_Player.position);

        // Only shoot if within range and enough time has passed
        if (distanceToPlayer <= shootRange && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Shoot();
        }
    }

    // Instantiate and fire a bullet
    private void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            // Spawn bullet prefab at fire point
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

            // Apply velocity to bullet if it has a Rigidbody
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // NOTE: Unity uses rb.velocity (not rb.linearVelocity) for movement
                rb.linearVelocity = firePoint.forward * bulletSpeed;
            }

            // Destroy bullet after 5 seconds to clean up
            Destroy(bullet, 5f);
        }
    }

    // Choose a random patrol point from the list and go there
    private void ChooseRandomPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;

        currentPatrolIndex = Random.Range(0, patrolPoints.Length);
        m_NavAgent.SetDestination(patrolPoints[currentPatrolIndex].position);
        isBlocked = false;
    }

    // Called when a collision begins (placeholder)
    private void OnCollisionEnter(Collision collision)
    {
        // You can add logic here to handle what happens on collision
    }
}
