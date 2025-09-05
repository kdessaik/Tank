// ============================================================
// Script written by Yik Malehek
// Handles enemy tank shooting behaviour: 
// spawns shells, applies launch force, plays sound effects, 
// and controls shooting timing when the player enters range.
// ============================================================

using UnityEngine;

public class EnemyTankShooting : MonoBehaviour
{
    [Header("Shell Settings")]
    public Rigidbody m_Shell;                 // Prefab of the shell to shoot (assign in Inspector)
    public Transform m_FireTransform;         // Position and rotation where the shell is spawned
    public float m_LaunchForce = 30f;         // Launch speed/force applied to the shell

    [Header("Timing Settings")]
    public float m_ShootDelay = 1f;           // Time (seconds) between each shot
    public float m_InitialShootDelay = 3f;    // Initial delay before first shot after detection

    [Header("Audio Settings")]
    public AudioSource src;                   // Audio source for playing the shooting sound
    public AudioClip sfx1;                    // Shooting sound clip

    private bool m_CanShoot = false;          // Flag to check if tank can shoot
    private float m_ShootTimer;               // Timer to track when to shoot next
    private GameManagers gameManager;         // Reference to the GameManagers script to check game state

    // Called when the script instance is loaded
    private void Awake()
    {
        // Start the timer with the initial delay
        m_ShootTimer = m_InitialShootDelay;

        // Auto-assign FireTransform if not set in the Inspector (looks for a child named "FirePoint")
        if (m_FireTransform == null)
        {
            m_FireTransform = transform.Find("FirePoint");
        }
    }

    // Called before the first frame update
    private void Start()
    {
        // Find the GameManagers instance in the scene
        gameManager = FindObjectOfType<GameManagers>();
    }

    // Called once per frame
    private void Update()
    {
        // Stop shooting if there’s no gameManager or if the game isn’t in Playing state
        if (gameManager == null || gameManager.State != GameManagers.GameState.Playing)
            return;

        // Only count down and fire if shooting is enabled
        if (m_CanShoot)
        {
            // Decrease timer each frame
            m_ShootTimer -= Time.deltaTime;

            // When timer reaches zero, fire and reset timer
            if (m_ShootTimer <= 0f)
            {
                Fire();
                m_ShootTimer = m_ShootDelay; // Reset timer for the next shot
            }
        }
    }

    // Instantiates and launches a shell, also plays sound effect
    private void Fire()
    {
        // Safety check to ensure the game is still in Playing state
        if (gameManager.State != GameManagers.GameState.Playing)
            return;

        // Instantiate a new shell at fire point’s position and rotation
        Rigidbody shellInstance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation);

        // Apply velocity to the shell so it moves forward at the given force
        // ✅ Unity uses "velocity" (not "linearVelocity")
        shellInstance.linearVelocity = m_LaunchForce * m_FireTransform.forward;

        // Play shooting sound if AudioSource and Clip are assigned
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

    // Trigger enters when player enters detection zone
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && gameManager.State == GameManagers.GameState.Playing)
        {
            // Enable shooting and reset initial delay timer
            m_CanShoot = true;
            m_ShootTimer = m_InitialShootDelay;
        }
    }

    // Trigger exits when player leaves detection zone
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Stop shooting immediately
            m_CanShoot = false;
        }
    }

    // Public method called by GameManager to reset or disable shooting
    public void ResetShooting()
    {
        m_CanShoot = false;
        m_ShootTimer = m_InitialShootDelay;
    }

    // Public method to enable/disable shooting externally
    public void SetShootingEnabled(bool enabled)
    {
        if (!enabled)
        {
            m_CanShoot = false; // Disable immediately
        }
    }
}
