using Cinemachine;
using UnityEngine;
using Zeigblair.Core.Singletons;

public class PlayerCameraFollow : NetworkSingleton<PlayerCameraFollow>
{
    private CinemachineVirtualCamera cinemachineVirtualCamera;
    private void Awake()
    {
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    public void FollowPlayer(Transform transform)
    {
        cinemachineVirtualCamera.Follow = transform;
    }
}
