using UnityEngine;

public class playerScript : MonoBehaviour
{
    public Rigidbody rb;
    public float speed = 3f;
    public float m_RotationSpeed = 180f;//how fast the tank turns in degrees per second

    public Rigidbody m_Rigidbody;

    
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
        
        m_TurnInputValue = 0f;
    }
    private void OnDisable()
    {
        //when the tank is turned off, make it kinematic so it stops moving
        m_Rigidbody.isKinematic = true;
    }


    private void FixedUpdate()
    {
        Update();
        Turn();
    }




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_TurnInputValue = Input.GetAxis("Horizontal");

        if (Input.GetKey(KeyCode.W))
        {


            if (Input.GetKey(KeyCode.A))
            {
                rb.linearVelocity = new Vector3(-speed, rb.linearVelocity.y, speed);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                rb.linearVelocity = new Vector3(speed, rb.linearVelocity.y, speed);
            }
            else
            {
                rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, speed);
            }
        }

        else if (Input.GetKey(KeyCode.A))
        {
            rb.linearVelocity = new Vector3(-speed, rb.linearVelocity.y, 0);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            rb.linearVelocity = new Vector3(speed, rb.linearVelocity.y, 0);
        }


        
        else if (Input.GetKey(KeyCode.S))
        {
            

            if (Input.GetKey(KeyCode.A))
            {
                rb.linearVelocity = new Vector3(-speed, rb.linearVelocity.y, -speed);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                rb.linearVelocity = new Vector3(speed, rb.linearVelocity.y, -speed);
            }
            else
            {
                rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, -speed);
            }



        }
        else if (Input.GetKey(KeyCode.A))
        {
            rb.linearVelocity = new Vector3(-speed, rb.linearVelocity.y, 0);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            rb.linearVelocity = new Vector3(speed, rb.linearVelocity.y, 0  );
        }
        else
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }


    private void Turn()
    {
        //Determine the number of degrees to be turned based on the input,
        //speed and time between frames
        float turnValue = m_TurnInputValue * m_RotationSpeed * Time.deltaTime;

        //Make this into a rotation in the y axis
        Quaternion turnRotation = Quaternion.Euler(0f, turnValue, 0f);

        //Apply this rotation to the rigidbody's rotation
        m_Rigidbody.MoveRotation(transform.rotation * turnRotation);
    }


}
