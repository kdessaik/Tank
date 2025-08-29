using System.Collections;
using UnityEngine;

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

    [Header("Respawn Settings (For Enemies Only)")]
    public bool shouldRespawn = true;
    public float respawnDelay = 3f;
    public GameObject enemyTankPrefab;
    public Transform[] spawnPoints;

    private float currentHealth;
    private bool isDead;
    private ParticleSystem explosionParticles;

    private void Awake()
    {
        // Instantiate explosion effect and keep it disabled
        if (explosionPrefab != null)
        {
            GameObject explosionInstance = Instantiate(explosionPrefab);
            explosionParticles = explosionInstance.GetComponent<ParticleSystem>();
            explosionParticles.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        currentHealth = startingHealth;
        isDead = false;
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        Debug.Log(gameObject.name + " Health: " + currentHealth);

        if (currentHealth <= 0f && !isDead)
        {
            OnDeath();
        }
    }

    private void OnDeath()
    {
        isDead = true;

        // Play explosion effect at tank's position
        if (explosionParticles != null)
        {
            explosionParticles.transform.position = transform.position;
            explosionParticles.gameObject.SetActive(true);
            explosionParticles.Play();
        }

        // Start death routine
        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(3f); // Allow explosion animation to play

        gameObject.SetActive(false); // Disable tank

        // Handle respawn for enemies only
        if (!isPlayer && shouldRespawn && enemyTankPrefab != null)
        {
            yield return new WaitForSeconds(respawnDelay);
            RespawnEnemy();
        }
        else if (isPlayer)
        {
            // Game Over logic for player
            Debug.Log("GAME OVER!");
            // You can load a Game Over screen or restart level here
        }
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

    // Optional: Method to heal tank
    public void Heal(float amount)
    {
        if (!isDead)
        {
            currentHealth = Mathf.Min(currentHealth + amount, startingHealth);
           
        }
    }
}
