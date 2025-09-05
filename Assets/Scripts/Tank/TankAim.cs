// ============================================================
// Script written by Esther Namulen
// Tank aiming script: rotates the turret to aim where the mouse points
// ============================================================

using UnityEngine;

public class TankAim : MonoBehaviour
{
    // Reference to the turret transform (the rotating part of the tank)
    public Transform m_Turret;

    // LayerMask used to filter what the raycast can hit (e.g., only the ground)
    private LayerMask m_layerMask;

    // Called when the script instance is loaded into the scene
    private void Awake()
    {
        // Get the LayerMask for the layer named "Ground"
        // This ensures the raycast only detects the ground and not other objects
        m_layerMask = LayerMask.GetMask("Ground");
    }

    // Start is called before the first frame update
    void Start()
    {
        // Nothing is needed here for now, but this is where initialization can be placed
    }

    // Called once per frame
    void Update()
    {
        // Create a ray from the main camera through the mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Store information about what the raycast hits
        RaycastHit hit;

        // Shoot a raycast to detect where the mouse is pointing on the ground
        // If the ray hits something in the "Ground" layer, store it in hit
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, m_layerMask))
        {
            // Rotate the turret so it looks at the hit point on the ground
            m_Turret.LookAt(hit.point);
        }
    }
}
