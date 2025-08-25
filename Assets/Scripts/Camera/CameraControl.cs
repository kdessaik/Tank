using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float m_DampTime = 0.5f;


    public Transform m_target;
    private Vector3 m_MoveVelocity;
    private Vector3 m_DesiredPosition;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        //Remove the manually add target to follow instead
        m_target = GameObject.FindGameObjectWithTag("Player").transform;
       
}

    void Start()
    {
         
    }

    // Update is called once per frame
    void Update()
    {
       float scroll = Input.GetAxis("Mouse ScrollWheel");
        

        Camera.main.orthographicSize -= scroll * 10000 * Time.deltaTime;
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 10, 60);

        Move(); 
    }
    private void Move()
    {
        m_DesiredPosition = m_target.position;
        transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);
        
    }
}
