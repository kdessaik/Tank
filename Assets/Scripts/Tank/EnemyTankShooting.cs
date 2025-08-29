using UnityEngine;

public class EnemyTankShooting : MonoBehaviour
{
    // Prefab of the Shell
    public Rigidbody m_Shell;
    // A child of the tank where the shells are spawned
    public Transform m_FireTransform;

    // The force given to the shell when firing
    public float m_LaunchForce = 30f;

    // Time between each shot
    public float m_ShootDelay = 1f;

    // Initial delay before shooting starts
    public float m_InitialShootDelay = 3f;

    private bool m_CanShoot;
    private float m_ShootTimer;

    private void Awake()
    {
        m_CanShoot = false;
        m_ShootTimer = m_InitialShootDelay; // Start with 3 seconds before first shot
    }

    void Update()
    {
        if (m_CanShoot)
        {
            m_ShootTimer -= Time.deltaTime;

            if (m_ShootTimer <= 0f)
            {
                Fire();
                m_ShootTimer = m_ShootDelay; // Reset timer for the next shot
            }
        }
    }

    private void Fire()
    {
        // Create an instance of the shell and store a reference to its Rigidbody
        Rigidbody shellInstance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation);

        // Set the shell's velocity to the launch force in the fire position's forward direction
        shellInstance.linearVelocity = m_LaunchForce * m_FireTransform.forward;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            m_CanShoot = true;
            m_ShootTimer = m_InitialShootDelay; // Wait 3 seconds before shooting
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            m_CanShoot = false;
        }
    }
}
 