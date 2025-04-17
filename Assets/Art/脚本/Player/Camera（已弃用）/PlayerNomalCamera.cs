using System.Collections;
using UnityEngine;

public class PlayerNormalCamera : MonoBehaviour
{
    public PlayerManager playerManager;


    private Transform cameraTransform; //主相机
    private Transform cameraPivotTransform; //挂载该脚本的对象负责左右旋转，子物体负责上下旋转，主相机赋值防碰撞(z的范围控制)

    private Vector3 pivotOffset = new Vector3(0, 1.5f, 0); // cameraPivotTransform的偏移
    private Vector3 cameraOffset = new Vector3(0, 0, -3); // 主相机的偏移

    [Header("Camera Setting")]
    private float cameraSmoothSpeed = 0.05f; //相机移动平滑速度
    private float leftAndRightRotationSpeed = 200; //左右旋转速度
    private float upAndDownRotationSpeed = 200; //上下旋转速度
    private float minimumPiovt = -30; //上下最低旋转角度
    private float maximumPiovt = 60; //上下最高旋转角度
    private float cameraCollisionRadius = 0.2f; //圆形碰撞检测半径
    [SerializeField] private LayerMask collideWithLayers; //与相机碰撞层级
    private float smoothTime = 0.1f; // 控制平滑时间
    private Quaternion currentRotation;
    private Quaternion targetRotation;

    [Header("Camera Values")]
    private Vector3 cameraVelocity; //相机跟随时的平滑处理参考值
    private Vector3 cameraObjectPosition; //与碰撞物体接触时用需要调整位置，这个值用来存储对应值
    private float leftAndRightLookAngle; //左右旋转的角度
    private float upAndDownLookAngle; //上下旋转的角度
    private float cameraZPosition; //碰撞时用到
    private float targetCameraZPosition; //碰撞时用到

    private void Awake()
    {
        cameraPivotTransform = CameraManager.Instance.playerCameraManager.playerCameraPivotTransform;

        cameraTransform = CameraManager.Instance.playerCameraManager.playerMainCamera.transform;

        //设置相机相对于player的位置
        cameraPivotTransform.localPosition = pivotOffset;
        cameraTransform.transform.localPosition = cameraOffset;
    }

    private void OnEnable()
    {
        leftAndRightLookAngle = transform.rotation.eulerAngles.y;
        // 修正上下角度的初始化（将 0~360 转为 -180~180）
        float rawUpDownAngle = cameraPivotTransform.localRotation.eulerAngles.x;
        upAndDownLookAngle = (rawUpDownAngle > 180f) ? rawUpDownAngle - 360f : rawUpDownAngle;
        cameraVelocity = Vector3.zero;
    }
    private void Start()
    {
        cameraZPosition = cameraTransform.transform.localPosition.z; //开始时记录相机的z，如果第一时间发生碰撞，那么就会进行对应的碰撞处理
    }


    private void LateUpdate()
    {
        HandleAllCameraActions();
    }

    /// <summary>
    /// 处理相机所有行为
    /// </summary>
    public void HandleAllCameraActions()
    {
        HandleFollowTarget();
        HandleRotations();
        HandleCollision();
    }

    /// <summary>
    /// 跟随处理（动态调整平滑速度）
    /// </summary>
    public void HandleFollowTarget()
    {
        // 动态计算 cameraSmoothSpeed 基于 yVelocity
        float minSpeed = 0.03f;
        float maxSpeed = 0.08f;
        float maxVelocityThreshold = 20f;
        float safeVelocityThreshold = 10f;

        // 计算平滑速度（基于 yVelocity 的绝对值）
        float absYVelocity = Mathf.Abs(playerManager.yVelocity.y);
        if (absYVelocity <= safeVelocityThreshold)
        {
            // yVelocity ∈ [-10, 10] → cameraSmoothSpeed = 0.08
            cameraSmoothSpeed = maxSpeed;
        }
        else if (absYVelocity >= maxVelocityThreshold)
        {
            // yVelocity ≤ -20 或 ≥ 20 → cameraSmoothSpeed = 0.01
            cameraSmoothSpeed = minSpeed;
        }
        else
        {
            // yVelocity ∈ (-20, -10) 或 (10, 20) → 线性插值
            float t = Mathf.InverseLerp(safeVelocityThreshold, maxVelocityThreshold, absYVelocity);
            cameraSmoothSpeed = Mathf.Lerp(maxSpeed, minSpeed, t);
        }

        // 平滑跟随角色位置
        Vector3 targetCameraPosition = Vector3.SmoothDamp(
            transform.position,
            playerManager.transform.position,
            ref cameraVelocity,
            cameraSmoothSpeed
        );
        transform.position = targetCameraPosition;
    }

    /// <summary>
    /// 旋转处理
    /// </summary>
    public void HandleRotations()
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

    /// <summary>
    /// 碰撞处理
    /// </summary>
    private void HandleCollision()
    {
        targetCameraZPosition = cameraZPosition;
        RaycastHit hit;
        Vector3 direction = cameraTransform.transform.position - cameraPivotTransform.position;
        direction.Normalize();

        if (Physics.SphereCast(cameraPivotTransform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetCameraZPosition), collideWithLayers))
        {
            float distanceFromHitObject = Vector3.Distance(cameraPivotTransform.position, hit.point);
            targetCameraZPosition = -(distanceFromHitObject - cameraCollisionRadius);
        }

        if (Mathf.Abs(targetCameraZPosition) < cameraCollisionRadius)
        {
            targetCameraZPosition = -cameraCollisionRadius;
        }

        cameraObjectPosition.z = Mathf.Lerp(cameraTransform.transform.localPosition.z, targetCameraZPosition, 0.2f);
        cameraTransform.transform.localPosition = cameraObjectPosition;
    }

    //震动设置"
    private float shakeFrequency = 25f; // 控制 Perlin 噪声采样频率，值越高变化越快
    private float shakeRotationFactor = 0.1f;// 控制旋转幅度（相对于位置幅度的乘数）

    private bool isShaking = false;
    private Vector3 originalCameraPosition; // 初始局部位置
    private Quaternion originalCameraRotation; // 初始局部旋转
    private Vector3 noiseSeed; // 用于 Perlin 噪声的随机种子

    /// <summary>
    /// 触发一次相机震动
    /// </summary>
    /// <param name="duration">震动持续时间</param>
    /// <param name="magnitude">最大震动幅度</param>
    public void StartCameraShake(float duration, float magnitude)
    {
        if (isShaking) return;

        // 记录初始位置与旋转
        originalCameraPosition = cameraTransform.localPosition;
        originalCameraRotation = cameraTransform.localRotation;

        // 生成三个轴向的噪声种子
        noiseSeed = new Vector3(Random.value, Random.value, Random.value) * 100f;

        StartCoroutine(DoPerlinShake(duration, magnitude));
    }
    private IEnumerator DoPerlinShake(float duration, float initialMagnitude)
    {
        isShaking = true;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;

            // 立方衰减曲线（可替换为其他曲线）
            float attenuation = Mathf.Pow(1 - progress, 3);
            float currentMagnitude = initialMagnitude * attenuation;

            // 三维Perlin噪声 三个方向的 Perlin 噪声值 [-1, 1]
            float x = (Mathf.PerlinNoise(noiseSeed.x, Time.time * shakeFrequency) * 2 - 1);
            float y = (Mathf.PerlinNoise(noiseSeed.y, Time.time * shakeFrequency) * 2 - 1);
            float z = (Mathf.PerlinNoise(noiseSeed.z, Time.time * shakeFrequency) * 2 - 1);

            // 应用位置偏移
            Vector3 positionOffset = new Vector3(x, y, z) * currentMagnitude;
            cameraTransform.localPosition = originalCameraPosition + positionOffset;

            // 应用旋转偏移（可选）
            if (shakeRotationFactor > 0)
            {
                Vector3 rotationOffset = new Vector3(y, x, z) * currentMagnitude * shakeRotationFactor;
                cameraTransform.localRotation = originalCameraRotation * Quaternion.Euler(rotationOffset);
            }

            yield return null;
        }

        // 精准还原初始状态
        cameraTransform.localPosition = originalCameraPosition;
        cameraTransform.localRotation = originalCameraRotation;
        isShaking = false;
    }

}