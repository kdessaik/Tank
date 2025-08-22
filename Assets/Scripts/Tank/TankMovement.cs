using UnityEngine;

public class TankMovement : MonoBehaviour
{
    public float m_speed = 20f;//how fast the tank moves forward and back
    public float m_RotationSpeed = 180f;//how fast the tank turns in degrees per second

    public Rigidbody m_Rigidbody;

    private float m_ForwardInputValue;//the current value of the forward input
    private float m_TurnInputValue;//the current value of the turn input


    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        //when the tank is turned on, make sure it's not kinematic
        m_Rigidbody.isKinematic = false;

        //also reset the input values
        m_ForwardInputValue = 0;
        m_TurnInputValue = 0;
    }
    private void OnDisable()
    {
        //when the tank is turned off, make it kinematic so it stops moving
        m_Rigidbody.isKinematic = true;
    }
    private void Update()
    {
        m.ForwardInputValue = Input.GetAxis("Vertical");
        m_TurnInputValue = Input.GetAxis("Horizontal");
    }
    private void Move()
    {
        //create a vector in the direction the tank is facing with a magnitude
        // Based on the input, speed and time between frames
        Vector3 wantedVelocity = transform.forward * m_ForwardInputValue * m_speed ;

        //Apply the wantedVelocity minus the current  rigidbody velocity to apply a change
        // in the velocity on the tank
        //this ignores the mass of the tank
        m_Rigidbody.AddForce(wantedVelocity - m_Rigidbody.velocity, ForceMode.VelocityChange);



    }
    private void Move()
            {
        //Create a vector in the direction the tank is facing with a magnitude
        //based on the input, speed and time between frames
        Vector3 wantedVelocity = transform.forward * m_ForwardInputValue * m_speed;

        //Apply the wantedVelocity minus the current rigidbody velocity to apply a change
        //in the velocity on the tank
        //this ignores the mass of the tank
        m_Rigidbody.AddForce(wantedVelocity - m_Rigidbody.velocity, ForceMode.VelocityChange);

    }
    private void Turn()
    {
        //Determine the number of degrees to be turned based on the input,
        //speed and time between frames
        float turn = m_TurnInputValue * m_RotationSpeed * Time.deltaTime;

        //Make this into a rotation in the y axis
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);

        //Apply this rotation to the rigidbody's rotation
        m_Rigidbody.MoveRotation(transofrm.rotation * turnRotation);
    }

    
}
