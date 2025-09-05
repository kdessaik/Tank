// ============================================================
// Script written by Esther NAMULEN
// Smoothly follows the player with damping and allows
// zooming the orthographic camera using the mouse scroll wheel.
// ============================================================

using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [Header("Camera Settings")]
    public float m_DampTime = 0.5f; // Smooth movement time

    [Header("Target")]
    public Transform m_target;      // Target to follow automatically

    private Vector3 m_MoveVelocity;
    private Vector3 m_DesiredPosition;

    private void Awake()
    {
        // Automatically find and assign the player as target
        m_target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        // Handle zoom with mouse wheel
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        Camera.main.orthographicSize -= scroll * 10000 * Time.deltaTime;
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 10, 60);

        Move();
    }

    private void Move()
    {
        if (m_target == null) return;

        m_DesiredPosition = m_target.position;
        transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);
    }
}
