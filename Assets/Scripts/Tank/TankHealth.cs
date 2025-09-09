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
    public bool isPlayer = false;

    [Tooltip("Starting health for this tank.")]
    public float startingHealth = 100f;

    [Tooltip("Damage taken per hit.")]
    public float damagePerHit = 10f;

    [Header("Explosion Effect")]
    public GameObject explosionPrefab;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip deathClip, explosionClip;
    public AudioClip victoryClip;

    [Header("Respawn Settings (For Enemies Only)")]
    public bool shouldRespawn = true;
    public float respawnDelay = 2f;
    public GameObject enemyTankPrefab;
    public Transform[] spawnPoints;

    private float currentHealth;
    private bool isDead;
    private ParticleSystem explosionParticles;

    [Header("Health UI")]
    public Slider m_Slider;
    public Image m_FillImage;
    public Color m_FullHealthColor = Color.green;
    public Color m_ZeroHealthColor = Color.red;

    private void Awake()
    {
        if (explosionPrefab != null)
        {
            GameObject explosionInstance = Instantiate(explosionPrefab);
            explosionParticles = explosionInstance.GetComponent<ParticleSystem>();
            if (explosionParticles != null)
            {
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

    private void OnEnable()
    {
        currentHealth = startingHealth;
        isDead = false;

        if (m_Slider != null)
        {
            m_Slider.maxValue = startingHealth;
            m_Slider.value = currentHealth;
        }

        SetHealthUI();

        if (isPlayer && currentHealth <= 0f)
        {
            TriggerGameOver();
        }
    }

    /// <summary>
    /// Updates the health bar UI.
    /// </summary>
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

    /// <summary>
    /// Plays the victory sound if available.
    /// </summary>
    public void PlayVictorySound()
    {
        PlayAudioClip(victoryClip, true);
    }

    /// <summary>
    /// Applies damage to the tank.
    /// </summary>
    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        SetHealthUI();

        if (currentHealth <= 0f && !isDead)
        {
            OnDeath();
        }
    }

    /// <summary>
    /// Handles tank death effects, sounds, and respawn logic.
    /// </summary>
    private void OnDeath()
    {
        isDead = true;

        var enemyShooting = GetComponent<EnemyTankShooting>();
        if (enemyShooting != null)
        {
            enemyShooting.SetShootingEnabled(false);
        }

        if (explosionParticles != null)
        {
            explosionParticles.transform.position = transform.position;
            explosionParticles.gameObject.SetActive(true);

            // Play explosion effect twice
            StartCoroutine(PlayParticlesTwice(explosionParticles));
        }
        else
        {
            Debug.LogWarning("No explosion effect available for " + gameObject.name);
        }

        // Play death and explosion sounds
        PlayAudioClip(deathClip);
        PlayAudioClip(explosionClip, false);

        StartCoroutine(DeathRoutine());
    }

    /// <summary>
    /// Plays a given audio clip.
    /// </summary>
    private void PlayAudioClip(AudioClip clip, bool oneShot = true)
    {
        if (audioSource == null || clip == null) return;

        if (oneShot)
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    /// <summary>
    /// Plays the particle system twice with proper delay.
    /// </summary>
    private IEnumerator PlayParticlesTwice(ParticleSystem particleSystem)
    {
        var mainModule = particleSystem.main;
        mainModule.loop = false;

        // First play
        particleSystem.Play();
        yield return new WaitForSeconds(particleSystem.main.duration);

        // Second play
        particleSystem.Play();
    }

    private IEnumerator DeathRoutine()
    {
        float delay = (deathClip != null) ? deathClip.length : 3f;
        yield return new WaitForSeconds(delay);

        gameObject.SetActive(false);

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

    private void TriggerGameOver()
    {
        Debug.Log("GAME OVER!");
        // TODO: Show UI or restart game here
    }

    private void RespawnEnemy()
    {
        Vector3 spawnPosition;
        Quaternion spawnRotation = Quaternion.identity;

        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            spawnPosition = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
        }
        else
        {
            spawnPosition = transform.position;
        }

        GameObject newEnemy = Instantiate(enemyTankPrefab, spawnPosition, spawnRotation);

        var enemyMovement = newEnemy.GetComponent<EnemyTankMovement>();
        if (enemyMovement != null)
        {
            enemyMovement.m_Player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    /// <summary>
    /// Heals the tank by the given amount.
    /// </summary>
    public void Heal(float amount)
    {
        if (!isDead)
        {
            currentHealth = Mathf.Min(currentHealth + amount, startingHealth);
            SetHealthUI();
        }
    }
}
