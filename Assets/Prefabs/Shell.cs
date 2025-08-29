using UnityEngine;

public class Shell : MonoBehaviour
{
    //The time in second before the shell is removed
    public float m_MaxLifeTime = 2f;
    //The amount of the damage done if the explosion is centered on a tank
    public float m_MaxDamage = 10f;
    //The maximum distance away from the explosion tanks can be and are still affected
    public float m_ExplosionRadius = 5f;
    //The amount of force added to a tank at the center of the explosion
    public float m_ExplosionForce = 100f;

    //Reference to the partcle that wil play on explosion
    public ParticleSystem m_ExplosionParticles;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //If it isn't destroyed by then, destroy the shell after it's lifetime
        Destroy(gameObject, m_MaxLifeTime);
    }
    private void OnCollisionEnter(Collision other)
    {
        //find the rigidbody of the collision object
        Rigidbody targetRigidbody = other.gameObject.GetComponent<Rigidbody>();
        //only tanks will have rigidbody scripts

        if (targetRigidbody != null)
        {
            //Add an explosion force
            targetRigidbody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);

            //find the TankHealth script associated with the rigidbody
            TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth>();
            if (targetHealth != null)
            {
                //calculate the amount of damage the target should take 
                //based on it's distance from the shell
                float damage = CalculateDamage(targetRigidbody.position);

                //Deal this damage to the tank
                targetHealth.TakeDamage(damage);
            }
        }

        

        //Unparent the particles from the shell
        m_ExplosionParticles.transform.parent =null;
        //Play the particle system
        m_ExplosionParticles.Play();

        //Once the particles have finished, destroy the gameObject they are on 
        Destroy(m_ExplosionParticles.gameObject, m_ExplosionParticles.main.duration);

        //Destroy the shell
        Destroy(gameObject);


    }
    private float CalculateDamage(Vector3 targetPosition)
    {
        //Crete a vector from the shell to the target
        Vector3 explosionToTarget = targetPosition - transform.position;

        //Calculate the distance from the shell to the target
        float explosionDistance = explosionToTarget.magnitude;
        //Calculate the proportion of the maximum distance (the explosionRadius)
        //the target is way
        float relativeDistance = (m_ExplosionRadius * explosionDistance) / m_ExplosionRadius;
        //Calculate damgw as this proportion of the maximum possible damage
        float damage = relativeDistance * m_MaxDamage;

        //Make sure that the minimum damge is always 0
        damage = Mathf.Max(0f, damage);
        return damage;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
