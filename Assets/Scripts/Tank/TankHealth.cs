using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TankHealth : MonoBehaviour
{
    [Header("Tank Settings")]
    [Tooltip("Set true if this is the player tank.")]
    public bool isPlayer = false;

    [Tooltip("Starting health for this tank.")]
    public float startingHealth = 1000f;

    [Tooltip("Damage taken per hit.")]
    public float damagePerHit = 100f;

    [Header("Explosion Effect")]
    public GameObject explosionPrefab;

    [Header("Audio Settings")]
    public AudioSource audioSource;       // Audio source for sound effects
    public AudioClip deathClip, explosionClip; // Death and explosion sounds

    [Header("Respawn Settings (For Enemies Only)")]
    public bool shouldRespawn = true;
    public float respawnDelay = 3f;
    public GameObject enemyTankPrefab;
    public Transform[] spawnPoints;

    private float currentHealth;
    private bool isDead;
    private ParticleSystem explosionParticles;

    [Header("Health UI")]
    public Slider m_Slider;                  // The UI Slider for health
    public Image m_FillImage;                // The Image to change color
    public Color m_FullHealthColor = Color.green;
    public Color m_ZeroHealthColor = Color.red;

    public AudioClip victoryClip; // ✅ New victory sound

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

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        Debug.Log(gameObject.name + " Health: " + currentHealth);

        SetHealthUI();

        if (currentHealth <= 0f && !isDead)
        {
            OnDeath();
        }
    }

    private void OnDeath()
    {
        isDead = true;

        // Stop enemy shooting if it's an enemy
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

        // Play death sound
        if (audioSource != null && deathClip != null)
        {
            audioSource.PlayOneShot(deathClip);
        }
        if (audioSource != null && explosionClip != null)
        {
            audioSource.clip = explosionClip;
            audioSource.Play();
        }

        StartCoroutine(DeathRoutine());
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

        EnemyTankMovement enemyMovement = newEnemy.GetComponent<EnemyTankMovement>();
        if (enemyMovement != null)
        {
            enemyMovement.m_Player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    public void Heal(float amount)
    {
        if (!isDead)
        {
            currentHealth = Mathf.Min(currentHealth + amount, startingHealth);
            SetHealthUI();
        }
    }
}
