using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(Collider2D))] // Ensures a 2D Collider is attached
public class CameraZone : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] CinemachineVirtualCameraBase zoneCamera;

    CinemachineVirtualCameraBase previousCamera;

    void Start()
    {
        // Debug: Check if Collider2D is attached
        if (GetComponent<Collider2D>() == null)
        {
            Debug.LogError("No Collider2D component found on " + gameObject.name);
        }
        else
        {
            Debug.Log("Collider2D found on " + gameObject.name);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Debug.Log("Player entered trigger zone: " + gameObject.name);

        // Store current camera
        CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();
        if (brain != null && brain.ActiveVirtualCamera is CinemachineVirtualCameraBase activeCamera)
        {
            previousCamera = activeCamera;
        }

        // Switch to zone camera
        if (zoneCamera != null)
        {
            Debug.Log("Switching to camera: " + zoneCamera.name);
            CameraSwitcher.SwitchCamera(zoneCamera);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Debug.Log("Player exited trigger zone: " + gameObject.name);

        // Return to previous camera
        if (previousCamera != null)
        {
            Debug.Log("Switching back to camera: " + previousCamera.name);
            CameraSwitcher.SwitchCamera(previousCamera);
        }
    }

    void OnValidate()
    {
        // Ensure the collider is set as a trigger
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }
    }
}