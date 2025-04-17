using UnityEngine;

//空中蓄力相机，角色在蓄力过程不会有位移变化，因此只需要处理旋转
public class PlayerAirChargeCamera : MonoBehaviour
{
    //旋转设置
    private float leftAndRightRotationSpeed = 200; //左右旋转速度
    private float upAndDownRotationSpeed = 200; //上下旋转速度
    private float minimumPiovt = -30; //上下最低旋转角度
    private float maximumPiovt = 60; //上下最高旋转角度
    private float smoothTime = 0.1f; // 控制旋转平滑时间
    private float pointSmoothtime = 0.5f; //控制距离平滑
    private Quaternion currentRotation;
    private Quaternion targetRotation;
    private float leftAndRightLookAngle; //左右旋转的角度
    private float upAndDownLookAngle; //上下旋转的角度

    //位置平滑设置
    private Vector3 currentCameraPosition;
    private Vector3 moveVelocity = Vector3.zero;

    //初始设置
    private Transform cameraPivotTransform;
    private PlayerManager playerManager;

    private void Awake()
    {
        cameraPivotTransform = CameraManager.Instance.playerCameraManager.playerCameraPivotTransform;
        playerManager = CameraManager.Instance.playerCameraManager.playerManager;
    }

    private void OnEnable()
    {
        leftAndRightLookAngle = transform.rotation.eulerAngles.y;
        // 修正上下角度的初始化（将 0~360 转为 -180~180）
        float rawUpDownAngle = cameraPivotTransform.localRotation.eulerAngles.x;
        upAndDownLookAngle = (rawUpDownAngle > 180f) ? rawUpDownAngle - 360f : rawUpDownAngle;

        currentCameraPosition = transform.position;
    }


    private void LateUpdate()
    {
        HandleAllCameraActions();
    }

    public void HandleAllCameraActions()
    {
        HandleRotations();
        FarAwayPlayer();
    }

    // 逐渐远离角色（平滑移动）
    private void FarAwayPlayer()
    {
        Vector3 localOffset = new Vector3(0, 0, -1);
        Vector3 targetPosition = playerManager.transform.position + playerManager.transform.rotation * localOffset;

        currentCameraPosition = Vector3.SmoothDamp(
            currentCameraPosition,     // 当前平滑位置
            targetPosition,            // 目标位置
            ref moveVelocity,          // 平滑速度
            pointSmoothtime                 // 平滑时间
        );

        transform.position = currentCameraPosition;
    }


    //处理旋转
    private void HandleRotations()
    {
        upAndDownLookAngle -= (playerManager.playerInputManager.cameraVerticalInput * upAndDownRotationSpeed) * Time.deltaTime;
        upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minimumPiovt, maximumPiovt);

        leftAndRightLookAngle += (playerManager.playerInputManager.cameraHorizontalInput * leftAndRightRotationSpeed) * Time.deltaTime;

        //限制在范围（-180，180）
        if (leftAndRightLookAngle > 180)
        {
            leftAndRightLookAngle -= 360;
        }
        else if (leftAndRightLookAngle < -180f)
        {
            leftAndRightLookAngle += 360;
        }


        // 水平旋转
        targetRotation = Quaternion.Euler(0f, leftAndRightLookAngle, 0f); //创建一个四元数，表示沿y旋转leftAndRightLookAngle度
        currentRotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothTime); //以相机当前的角度，沿y轴旋转leftAndRightLookAngle度
        transform.rotation = currentRotation;

        // 垂直旋转
        targetRotation = Quaternion.Euler(upAndDownLookAngle, 0f, 0f);
        cameraPivotTransform.localRotation = Quaternion.Slerp(cameraPivotTransform.localRotation, targetRotation, smoothTime);

    }
}
