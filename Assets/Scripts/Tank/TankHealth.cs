// ============================================================
// Script written by Dessai KIBEHO
// Handles health for both player and enemy tanks:
// tracks damage, death, respawn, explosion effects, and UI updates.
// ============================================================

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TankHealth : MonoBehaviour
{
    [Header("Tank Settings")]
    [Tooltip("Set true if this is the player tank.")]
    public bool isPlayer = false;           // Flag to identify player tank vs. enemy tank

    [Tooltip("Starting health for this tank.")]
    public float startingHealth = 1000f;    // Initial health value

    [Tooltip("Damage taken per hit.")]
    public float damagePerHit = 100f;       // Default damage per hit

    [Header("Explosion Effect")]
    public GameObject explosionPrefab;      // Prefab for explosion effect when tank dies

    [Header("Audio Settings")]
    public AudioSource audioSource;         // Audio source to play sounds
    public AudioClip deathClip, explosionClip; // Death and explosion sound clips

    [Header("Respawn Settings (For Enemies Only)")]
    public bool shouldRespawn = true;       // Should enemy tanks respawn after death
    public float respawnDelay = 3f;         // Delay before respawn
    public GameObject enemyTankPrefab;      // Enemy prefab used for respawn
    public Transform[] spawnPoints;         // Possible spawn points for respawned enemies

    private float currentHealth;            // Tracks current health
    private bool isDead;                    // Flag for whether this tank is dead
    private ParticleSystem explosionParticles; // Cached explosion particle system

    [Header("Health UI")]
    public Slider m_Slider;                 // UI slider representing health
    public Image m_FillImage;               // Fill image to change color based on health
    public Color m_FullHealthColor = Color.green; // Color when health is full
    public Color m_ZeroHealthColor = Color.red;   // Color when health is zero

    public AudioClip victoryClip;           // New victory sound clip

    // Called when the script instance is loaded
    private void Awake()
    {
        // If explosionPrefab assigned, create an instance and cache the particle system
        if (explosionPrefab != null)
        {
            GameObject explosionInstance = Instantiate(explosionPrefab);
            explosionParticles = explosionInstance.GetComponent<ParticleSystem>();
            if (explosionParticles != null)
            {
                // Disable the particle system until needed
                explosionParticles.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogWarning("Explosion prefab does not have a ParticleSystem: " + explosionPrefab.name);
            }
        }
        else
        {
            Debug.LogWarning("No explosionPrefab assigned to " + gameObject.name);
        }
    }

    // Called when object becomes enabled
    private void OnEnable()
    {
        // Reset health and state when tank is enabled
        currentHealth = startingHealth;
        isDead = false;

        // Update slider max value and current value
        if (m_Slider != null)
        {
            m_Slider.maxValue = startingHealth;
            m_Slider.value = currentHealth;
        }

        // Refresh UI color and value
        SetHealthUI();

        // If this is player and health is already zero, trigger game over
        if (isPlayer && currentHealth <= 0f)
        {
            TriggerGameOver();
        }
    }

    // Update UI slider and fill image based on current health
    private void SetHealthUI()
    {
        if (m_Slider != null)
        {
            m_Slider.value = currentHealth;
        }

        if (m_FillImage != null)
        {
            m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, currentHealth / startingHealth);
        }
    }

    // Plays a victory sound if assigned
    public void PlayVictorySound()
    {
        if (audioSource != null && victoryClip != null)
        {
            audioSource.PlayOneShot(victoryClip);
            Debug.Log("Victory sound played!");
        }
        else
        {
            Debug.LogWarning("Victory sound or AudioSource is missing!");
        }
    }

    // Call this to apply damage to the tank
    public void TakeDamage(float amount)
    {
        // Ignore if already dead
        if (isDead) return;

        currentHealth -= amount;
        Debug.Log(gameObject.name + " Health: " + currentHealth);

        // Update UI
        SetHealthUI();

        // Trigger death if health reaches zero
        if (currentHealth <= 0f && !isDead)
        {
            OnDeath();
        }
    }

    // Handles tank death: stops shooting, plays effects and sounds, starts death routine
    private void OnDeath()
    {
        isDead = true;

        // If this is an enemy, stop its shooting script
        EnemyTankShooting enemyShooting = GetComponent<EnemyTankShooting>();
        if (enemyShooting != null)
        {
            enemyShooting.SetShootingEnabled(false);
        }

        // Play explosion effect if available
        if (explosionParticles != null)
        {
            explosionParticles.transform.position = transform.position;
            explosionParticles.gameObject.SetActive(true);
            explosionParticles.Play();

            var mainExplosion = explosionParticles.main;
            mainExplosion.loop = true;
            Debug.Log("ExplosionParticles playing for " + gameObject.name);
        }
        else
        {
            Debug.LogWarning("No explosion effect available for " + gameObject.name);
        }

        // Play death and explosion sounds
        if (audioSource != null && deathClip != null)
        {
            audioSource.PlayOneShot(deathClip);
        }
        if (audioSource != null && explosionClip != null)
        {
            audioSource.clip = explosionClip;
            audioSource.Play();
        }

        // Start coroutine to handle disabling/respawning after a delay
        StartCoroutine(DeathRoutine());
    }

    // Waits for the death sound to finish, then disables or respawns tank
    private IEnumerator DeathRoutine()
    {
        float delay = (deathClip != null) ? deathClip.length : 3f;
        yield return new WaitForSeconds(delay);

        // Disable current tank object
        gameObject.SetActive(false);

        // If enemy tank, respawn; if player, trigger game over
        if (!isPlayer && shouldRespawn && enemyTankPrefab != null)
        {
            yield return new WaitForSeconds(respawnDelay);
            RespawnEnemy();
        }
        else if (isPlayer)
        {
            TriggerGameOver();
        }
    }

    // Handles game over logic for player tank
    private void TriggerGameOver()
    {
        Debug.Log("GAME OVER!");
        // TODO: Show UI or restart game here
    }

    // Spawns a new enemy tank at a random spawn point
    private void RespawnEnemy()
    {
        Vector3 spawnPosition;
        Quaternion spawnRotation = Quaternion.identity;

        // Choose random spawn point or use current position
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            spawnPosition = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
        }
        else
        {
            spawnPosition = transform.position;
        }

        // Instantiate new enemy tank
        GameObject newEnemy = Instantiate(enemyTankPrefab, spawnPosition, spawnRotation);

        // Assign player reference to the respawned enemy tank
        EnemyTankMovement enemyMovement = newEnemy.GetComponent<EnemyTankMovement>();
        if (enemyMovement != null)
        {
            enemyMovement.m_Player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    // Heals the tank by a given amount (clamped to max health)
    public void Heal(float amount)
    {
        if (!isDead)
        {
            currentHealth = Mathf.Min(currentHealth + amount, startingHealth);
            SetHealthUI();
        }
    }
}
