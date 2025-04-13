using UnityEngine;

public class CameraManager : MonoSingleton<CameraManager>
{
    public GameObject currentCameraObj;

    [Header("相机对象")]
    public PlayerCameraManager playerCameraManager;

    protected override void Init()
    {
        base.Init();

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        currentCameraObj = playerCameraManager.gameObject;
    }
}
