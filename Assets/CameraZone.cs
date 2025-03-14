using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(Collider2D))] 
public class CameraZone : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] CinemachineVirtualCameraBase zoneCamera;

    CinemachineVirtualCameraBase previousCamera;

    void Start()
    {
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;


        CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();
        if (brain != null && brain.ActiveVirtualCamera is CinemachineVirtualCameraBase activeCamera)
        {
            previousCamera = activeCamera;
        }

        if (zoneCamera != null)
        {
            CameraSwitcher.SwitchCamera(zoneCamera);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (previousCamera != null)
        {
            CameraSwitcher.SwitchCamera(previousCamera);
        }
    }

    void OnValidate()
    {
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }
    }
}