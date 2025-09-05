using UnityEngine;

public class EnemyTankShooting : MonoBehaviour
{
    [Header("Shell Settings")]
    public Rigidbody m_Shell;                 // Prefab of the Shell
    public Transform m_FireTransform;         // Position where the shell is spawned
    public float m_LaunchForce = 30f;         // Force given to the shell

    [Header("Timing Settings")]
    public float m_ShootDelay = 1f;           // Time between each shot
    public float m_InitialShootDelay = 3f;    // Delay before first shot

    [Header("Audio Settings")]
    public AudioSource src;                   // Audio source for shooting
    public AudioClip sfx1;                    // Shooting sound clip

    private bool m_CanShoot = false;
    private float m_ShootTimer;
    private GameManagers gameManager;

    private void Awake()
    {
        m_ShootTimer = m_InitialShootDelay;

        // Auto-assign FireTransform if not set in Inspector
        if (m_FireTransform == null)
        {
            m_FireTransform = transform.Find("FirePoint");
        }
    }
    private void Start()
    {
        gameManager = FindObjectOfType<GameManagers>();
    }

    private void Update()
    {
        // ✅ Stop shooting if GameManager is not set or game is not in Playing state
        if (gameManager == null || gameManager.State != GameManagers.GameState.Playing)
            return;

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
        // Extra safety check
        if (gameManager.State != GameManagers.GameState.Playing)
            return;

        // Instantiate shell
        Rigidbody shellInstance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation);
        shellInstance.linearVelocity = m_LaunchForce * m_FireTransform.forward;

        // Play shooting sound
        if (src != null && sfx1 != null)
        {
            src.clip = sfx1;
            src.Play();
        }
        else
        {
            Debug.LogWarning("EnemyTankShooting: AudioSource or Clip not assigned!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && gameManager.State == GameManagers.GameState.Playing)
        {
            m_CanShoot = true;
            m_ShootTimer = m_InitialShootDelay; // Reset delay before first shot
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            m_CanShoot = false;
        }
    }

    // ✅ Called by GameManager to reset or disable shooting
    public void ResetShooting()
    {
        m_CanShoot = false;
        m_ShootTimer = m_InitialShootDelay;
    }

    public void SetShootingEnabled(bool enabled)
    {
        if (!enabled)
        {
            m_CanShoot = false; // Disable immediately
        }
    }
}
