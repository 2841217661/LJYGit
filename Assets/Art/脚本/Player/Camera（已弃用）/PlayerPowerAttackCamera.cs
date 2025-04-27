using UnityEngine;

public class PlayerPowerAttackCamera : MonoBehaviour
{
    private PlayerManager playerManager; // 玩家管理器，用于获取玩家的位置与旋转

    [Header("Camera Settings")]
    private Vector3 localOffset = new Vector3(1.12f, -0.39f, -0.48f);
    // 相机在玩家本地坐标空间中的偏移量，决定相机相对于玩家的位置，例如位于左后方、稍微低一些

    private Vector3 rotationOffset = new Vector3(-11.9f, 120f, 0f);
    // 相机相对于玩家当前朝向的旋转偏移，比如从侧后方俯视角色脸部

    private Transform cameraPivotTransform; // 相机的中间旋转枢纽（用于处理上下旋转）

    private Vector3 endPosition;
    private Quaternion endRotation;

    [Header("Smoothing Curve")]
    private AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    // 默认就是先慢后快再慢（EaseInOut）
    private float transitionDuration = 7f; // 整个过渡过程持续时间
    private float transitionTimer = 0f;


    private void Awake()
    {
        // 从全局相机管理器中获取相机枢纽（cameraPivotTransform）的引用
        cameraPivotTransform = CameraManager.Instance.playerCameraManager.playerCameraPivotTransform;

        playerManager = CameraManager.Instance.playerCameraManager.playerManager;
    }

    private void OnEnable()
    {
        // 重置相机枢纽的本地旋转，防止旧的视角或过渡影响当前镜头角度
        cameraPivotTransform.localRotation = Quaternion.identity;

        // 计算相机目标世界坐标位置：
        // 将本地偏移量 localOffset 应用到玩家的世界坐标中
        Vector3 targetPosition = playerManager.transform.position +
                                 playerManager.transform.rotation * localOffset;

        // 计算相机目标世界旋转角度：
        // 将相机的本地旋转 offset（例如绕玩家侧面 120 度）应用到玩家的当前旋转上
        Quaternion targetRotation = playerManager.transform.rotation * Quaternion.Euler(rotationOffset);

        // 设置相机的位置和角度，使其从指定角度看向玩家
        transform.position = targetPosition;
        transform.rotation = targetRotation;

        endPosition = playerManager.transform.position;
        endRotation = playerManager.transform.rotation;

        // 重置相机初始状态
        transitionTimer = 0f;
    }

    private void LateUpdate()
    {
        transitionTimer += Time.deltaTime;
        float t = Mathf.Clamp01(transitionTimer / transitionDuration);
        float curvedT = transitionCurve.Evaluate(t); // 曲线控制插值速度

        transform.position = Vector3.Lerp(transform.position, endPosition, curvedT);
        transform.rotation = Quaternion.Slerp(transform.rotation, endRotation, curvedT);
    }

}
