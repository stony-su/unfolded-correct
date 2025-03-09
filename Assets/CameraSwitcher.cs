using System.Collections.Generic;
using Unity.Cinemachine;

public static class CameraSwitcher
{
    static List<CinemachineVirtualCameraBase> cameras = new List<CinemachineVirtualCameraBase>();

    public static CinemachineVirtualCameraBase ActiveCamera { get; private set; }

    public static void Register(CinemachineVirtualCameraBase camera)
    {
        cameras.Add(camera);
    }

    public static void Unregister(CinemachineVirtualCameraBase camera)
    {
        cameras.Remove(camera);
    }

    public static void SwitchCamera(CinemachineVirtualCameraBase camera)
    {
        camera.enabled = true;
        ActiveCamera = camera;

        foreach (var cam in cameras)
            if (cam != camera && cam.enabled)
                cam.enabled = false;
    }
}