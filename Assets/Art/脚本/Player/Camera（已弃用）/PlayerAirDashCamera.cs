using UnityEngine;

// ���������ͷŹ��󣬽��г�̣����ʱ�����ƶ��ӽ�
public class PlayerAirDashCamera : MonoBehaviour
{
    //��ʼ����
    private PlayerManager playerManager;

    //����
    private Vector3 currentCameraPosition;
    private float pointSmoothtime = 0.1f; // ���ƾ���ƽ��
    private float distance = 2f;


    private void Awake()
    {
        playerManager = CameraManager.Instance.playerCameraManager.playerManager;
    }

    private void OnEnable()
    {
        currentCameraPosition = transform.position;
        //��ȡ������ʱ�������Ұ�ı�
        CameraManager.Instance.playerCameraManager.ChangeCameraFOV(90f);
    }

    private void OnDisable()
    {
        //��ȡ������ʱ�������Ұ�ָ�
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

    // ƽ��Զ��player
    private void FarAwayPlayer()
    {
        Vector3 localOffset = new Vector3(0, 0, distance);
        Vector3 targetPosition = playerManager.transform.position + playerManager.transform.rotation * localOffset;

        // ʹ�� Lerp ���� SmoothDamp����Ӧ����
        currentCameraPosition = Vector3.Lerp(
            currentCameraPosition,
            targetPosition,
            Time.deltaTime / pointSmoothtime // ����ƽ���ٶȣ�ֵԽСԽ�죩
        );

        transform.position = currentCameraPosition;
    }

}
