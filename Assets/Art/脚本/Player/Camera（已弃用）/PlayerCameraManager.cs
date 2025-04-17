using UnityEngine;
using System.Collections;

public class PlayerCameraManager : MonoBehaviour
{

    private MonoBehaviour currentCameraContollerScript;

    [Header("设置")]
    public Camera playerMainCamera; //player主相机
    public PlayerManager playerManager; //玩家对象
    public Transform playerCameraPivotTransform; //相机支点，用于空中相机上下旋转


    [Header("Player相机对象")]
    public PlayerNormalCamera playerNormalCamera;
    public PlayerAirChargeCamera playerAirChargeCamera;
    public PlayerAirDashCamera playerAirDashCamera;
    public PlayerPowerAttackCamera playerPowerAttackCamera;


    private void Start()
    {
        //默认player使用相机normalCamera
        currentCameraContollerScript = playerNormalCamera;
    }

    //切换相机
    public void ChangePlayerCamera<T>(T _targetCameraControllerScript) where T : MonoBehaviour
    {
        currentCameraContollerScript.enabled = false; // 将当前的 player 相机控制脚本禁用
        _targetCameraControllerScript.enabled = true;  // 启用目标相机控制脚本
        currentCameraContollerScript = _targetCameraControllerScript;
    }

    [Header("改变相机视野")]
    private Coroutine fovCoroutine;
    private float fovChangeDuration = 0.3f;

    /// <summary>
    /// 启动协程，平滑过渡到目标 FOV
    /// </summary>
    public void ChangeCameraFOV(float targetFOV)
    {
        if (fovCoroutine != null)
        {
            StopCoroutine(fovCoroutine);
        }
        fovCoroutine = StartCoroutine(FOVTransition(playerMainCamera, targetFOV, fovChangeDuration));
    }

    private IEnumerator FOVTransition(Camera cam, float targetFOV, float duration)
    {
        float startFOV = cam.fieldOfView;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float newFOV = Mathf.Lerp(startFOV, targetFOV, t / duration);
            cam.fieldOfView = newFOV;
            yield return null;
        }

        cam.fieldOfView = targetFOV;
    }
}
