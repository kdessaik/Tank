// ============================================================
// Script written by Yik Malehek
// Handles the player tank shooting behaviour:
// spawns shells, applies launch force, and plays shooting sound.
// ============================================================

using UnityEngine;

public class TankShooting : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioSource src;           // Audio source used to play the shooting sound
    public AudioClip sfx1;             // Shooting sound clip

    [Header("Shell Settings")]
    public Rigidbody m_Shell;          // Prefab of the shell (assign in Inspector)
    public Transform m_FireTransform;  // Child transform where shells spawn (usually the barrel)

    // The force given to the shell when firing
    public float m_LauchForce = 30f;   // Original field (not used)
    private float m_LaunchForce = 30f; // Actual launch force used internally

    // Called before the first frame update
    void Start()
    {
        // If you want to sync m_LaunchForce to m_LauchForce automatically:
        m_LaunchForce = m_LauchForce;
    }

    // Called once per frame
    void Update()
    {
        // TODO: In the future, check if the game is over before firing
        // Fire when the Fire1 button (default: left mouse button) is released
        if (Input.GetButtonUp("Fire1"))
        {
            Fire();         // Spawn and shoot the shell
            src.clip = sfx1; // Assign the sound clip
            src.Play();      // Play the shooting sound
        }
    }

    // Spawns a shell prefab and applies launch force to it
    private void Fire()
    {
        // Create an instance of the shell at the fire transform�s position and rotation
        Rigidbody shellInstance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation);

        // Set the shell�s velocity so it moves forward at launch force
        // ? Unity uses "velocity" (not "linearVelocity")
        shellInstance.linearVelocity = m_LaunchForce * m_FireTransform.forward;
    }
}