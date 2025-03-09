using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCameraBase))]
public class RegisterCamera : MonoBehaviour
{
    private CinemachineVirtualCameraBase vcam;

    void Awake() => vcam = GetComponent<CinemachineVirtualCameraBase>();
    void OnEnable() => CameraSwitcher.Register(vcam);
    void OnDisable() => CameraSwitcher.Unregister(vcam);
}