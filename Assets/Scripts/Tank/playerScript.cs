using UnityEngine;

public class playerScript : MonoBehaviour
{
    public Rigidbody rb;
    public float speed = 1f;
    public GameObject trackRigth;
    public GameObject trackLeft;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
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
    }
