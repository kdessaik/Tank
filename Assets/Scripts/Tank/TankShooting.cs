using UnityEngine;

public class TankShooting : MonoBehaviour
{
    //Prefeb of the Shell
    public Rigidbody m_Shell;
    //A child of the tank where the shells are spawned
    public Transform m_FireTransform;
    //The forcegiven to the shell when firing
    public float m_LauchForce = 30f;
    private float m_LaunchForce = 30f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //ToDo: Later on,
        //sure the game isn't over
        if(Input.GetButtonUp("Fire1"))
        {
            Fire();
        }
        
    }
    private void Fire()
    {
        //Create an instance of the shell and store a reference to its rigidbody
        Rigidbody shellInstance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation);
        //Set the shell's velocity to the launch force in the fire 
        //position's forward direction
        shellInstance.linearVelocity = m_LaunchForce * m_FireTransform.forward;
    }
}
