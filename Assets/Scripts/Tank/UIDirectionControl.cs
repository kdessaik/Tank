// ============================================================
// Script written by Dessai KIBEHO
// Keeps UI elements (like world-space health bars) facing
// a consistent rotation relative to their parent.
// ============================================================

using UnityEngine;

public class UIDirectionControl : MonoBehaviour
{
    [Header("Rotation Settings")]
    // If true, the UI will keep the same rotation as its parent.
    // If false, it can rotate freely (e.g., face camera).
    public bool m_UseRelativeRotation = true;

    // Stores the rotation of the parent at start
    private Quaternion m_RelativeRotation;

    private void Start()
    {
        // Cache the parent's local rotation at startup
        if (transform.parent != null)
        {
            m_RelativeRotation = transform.parent.localRotation;
        }
    }

    private void Update()
    {
        // Apply the cached rotation each frame if enabled
        if (m_UseRelativeRotation)
        {
            transform.rotation = m_RelativeRotation;
        }
    }
}
