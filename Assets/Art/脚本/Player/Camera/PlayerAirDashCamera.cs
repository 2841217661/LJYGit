using UnityEngine;

// 空中蓄力释放过后，进行冲刺：冲刺时不能移动视角
public class PlayerAirDashCamera : MonoBehaviour
{
    //初始设置
    private PlayerManager playerManager;

    //设置
    private Vector3 currentCameraPosition;
    private float pointSmoothtime = 0.1f; // 控制距离平滑
    private float distance = 2f;


    private void Awake()
    {
        playerManager = CameraManager.Instance.playerCameraManager.playerManager;
    }

    private void OnEnable()
    {
        currentCameraPosition = transform.position;
        //在取消激活时将相机视野改变
        CameraManager.Instance.playerCameraManager.ChangeCameraFOV(90f);
    }

    private void OnDisable()
    {
        //在取消激活时将相机视野恢复
        CameraManager.Instance.playerCameraManager.ChangeCameraFOV(60f);
    }

    private void LateUpdate()
    {
        HandleAllCameraActions();
    }

    public void HandleAllCameraActions()
    {
        FarAwayPlayer();
    }

    // 平滑远离player
    private void FarAwayPlayer()
    {
        Vector3 localOffset = new Vector3(0, 0, distance);
        Vector3 targetPosition = playerManager.transform.position + playerManager.transform.rotation * localOffset;

        // 使用 Lerp 代替 SmoothDamp，响应更快
        currentCameraPosition = Vector3.Lerp(
            currentCameraPosition,
            targetPosition,
            Time.deltaTime / pointSmoothtime // 控制平滑速度（值越小越快）
        );

        transform.position = currentCameraPosition;
    }

}
