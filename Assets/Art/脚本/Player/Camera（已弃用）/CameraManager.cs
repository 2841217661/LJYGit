using UnityEngine;

public class CameraManager : MonoSingleton<CameraManager>
{
    public GameObject currentCameraObj;

    [Header("�������")]
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
