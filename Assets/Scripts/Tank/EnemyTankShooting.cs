using UnityEngine;

public class EnemyTankShooting : MonoBehaviour
{
    //Prefab of the Shell
    public Rigidbody m_Shell;
    //A child of the tank where the shells are spwned

    public Transform m_FireTransform;

    //The force given to the shell when when firing
    public float m_LaunchForce = 30f;
   

  
    public float m_ShootDelay = 1f;

    
    private bool m_CanShoot;
    private float m_ShootTimer;

    private void Awake()
    {
        m_CanShoot = false;
        m_ShootTimer = 0;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_CanShoot ==true)
        {
            m_ShootTimer -= Time.deltaTime;
            if(m_ShootTimer<=0)
            {
                m_ShootTimer = m_ShootDelay;
                Fire();
            }
        }
        
    }

    private void Fire()
    {
        //Create an instance of the shell and store a reference to its rigidbody
        Rigidbody shellInstance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;
        //Set the shell's velocity to the launch force in the fire 
        //position's forward direction
        shellInstance.linearVelocity = m_LaunchForce * m_FireTransform.forward;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag=="Player")
        {
            m_CanShoot = true;
            m_ShootTimer = m_ShootDelay; // Reset timer to shoot immediately upon player entry
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            m_CanShoot = false;
        }
    }
}
