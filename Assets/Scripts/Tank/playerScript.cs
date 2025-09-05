// ============================================================
// Script written by Esther Namulen
// Player movement and rotation script for Unity
// ============================================================

using UnityEngine;

public class playerScript : MonoBehaviour
{
    // Reference to the Rigidbody attached to the player
    public Rigidbody rb;

    // Movement speed of the player
    public float speed = 1f;

    // Rotation speed in degrees per second (how fast the player rotates)
    public float m_RotationSpeed = 180f;

    // Stores the current value of the forward/backward input (W/S keys)
    private float m_ForwardInputValue;

    // Reference to the player's Rigidbody used for physics-based movement
    public Rigidbody m_Rigidbody;

    // Stores the current value of the turn input (A/D keys)
    private float m_TurnInputValue;

    // Called when the script instance is being loaded
    private void Awake()
    {
        // Get the Rigidbody component attached to this GameObject
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    // Called when the object becomes enabled and active
    private void OnEnable()
    {
        // Ensure the Rigidbody is not kinematic so it can be affected by physics
        m_Rigidbody.isKinematic = false;

        // Reset the turn input value to zero
        m_TurnInputValue = 0f;
    }

    // Called when the object becomes disabled or inactive
    private void OnDisable()
    {
        // Make the Rigidbody kinematic so it stops moving and isn’t affected by physics
        m_Rigidbody.isKinematic = true;
    }

    // Called every fixed framerate frame (for physics updates)
    private void FixedUpdate()
    {
        // Process input and apply movement & rotation
        Update(); // Manually calling Update here (unusual, but in this script it’s used to capture input)
        Turn();
        Move();
    }

    // Called before the first frame update
    void Start()
    {
        // Get the collider of the player object tagged "Player"
        Collider playerCollider = GameObject.FindWithTag("Player").GetComponent<Collider>();

        // Get the collider attached to this GameObject
        Collider enemyCollider = GetComponent<Collider>();

        // Prevent collisions between this object and the player object
        Physics.IgnoreCollision(enemyCollider, playerCollider);
    }

    // Called once per frame
    void Update()
    {
        // Get horizontal input (A/D or arrow keys) for turning
        m_TurnInputValue = Input.GetAxis("Horizontal");

        // Get vertical input (W/S or arrow keys) for moving forward/backward
        m_ForwardInputValue = Input.GetAxis("Vertical");

        // Check movement keys manually to apply direct velocity changes to rb
        if (Input.GetKey(KeyCode.W))
        {
            // Moving forward with possible diagonal inputs
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
            // Move left
            rb.linearVelocity = new Vector3(-speed, rb.linearVelocity.y, 0);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            // Move right
            rb.linearVelocity = new Vector3(speed, rb.linearVelocity.y, 0);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            // Moving backward with possible diagonal inputs
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
            rb.linearVelocity = new Vector3(speed, rb.linearVelocity.y, 0);
        }
        else
        {
            // No key pressed, stop movement
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }

    // Handles movement using AddForce to change velocity
    private void Move()
    {
        // Create a vector pointing forward multiplied by input and speed
        Vector3 wantedVelocity = transform.forward * m_ForwardInputValue * speed;

        // Apply force to achieve the desired velocity instantly (ignoring mass)
        m_Rigidbody.AddForce(wantedVelocity - m_Rigidbody.linearVelocity, ForceMode.VelocityChange);
    }

    // Handles turning based on horizontal input
    private void Turn()
    {
        // Determine the amount of rotation this frame
        float turnValue = m_TurnInputValue * m_RotationSpeed * Time.deltaTime;

        // Create a rotation on the Y axis
        Quaternion turnRotation = Quaternion.Euler(0f, turnValue, 0f);

        // Apply this rotation to the Rigidbody
        m_Rigidbody.MoveRotation(transform.rotation * turnRotation);
    }
}
