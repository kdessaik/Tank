using UnityEngine;

public class TankAim : MonoBehaviour
{
    public Transform m_Turret;
    private LayerMask m_layerMask;

   
    



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        m_layerMask=LayerMask.GetMask("Ground");
        
    }
    void Start()
    {

        
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, m_layerMask))
        {
            m_Turret.LookAt(hit.point); 
        }
       

    }
}
