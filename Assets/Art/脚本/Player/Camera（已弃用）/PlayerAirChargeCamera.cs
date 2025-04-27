using UnityEngine;

//���������������ɫ���������̲�����λ�Ʊ仯�����ֻ��Ҫ������ת
public class PlayerAirChargeCamera : MonoBehaviour
{
    //��ת����
    private float leftAndRightRotationSpeed = 200; //������ת�ٶ�
    private float upAndDownRotationSpeed = 200; //������ת�ٶ�
    private float minimumPiovt = -30; //���������ת�Ƕ�
    private float maximumPiovt = 60; //���������ת�Ƕ�
    private float smoothTime = 0.1f; // ������תƽ��ʱ��
    private float pointSmoothtime = 0.5f; //���ƾ���ƽ��
    private Quaternion currentRotation;
    private Quaternion targetRotation;
    private float leftAndRightLookAngle; //������ת�ĽǶ�
    private float upAndDownLookAngle; //������ת�ĽǶ�

    //λ��ƽ������
    private Vector3 currentCameraPosition;
    private Vector3 moveVelocity = Vector3.zero;

    //��ʼ����
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
        // �������½Ƕȵĳ�ʼ������ 0~360 תΪ -180~180��
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

    // ��Զ���ɫ��ƽ���ƶ���
    private void FarAwayPlayer()
    {
        Vector3 localOffset = new Vector3(0, 0, -1);
        Vector3 targetPosition = playerManager.transform.position + playerManager.transform.rotation * localOffset;

        currentCameraPosition = Vector3.SmoothDamp(
            currentCameraPosition,     // ��ǰƽ��λ��
            targetPosition,            // Ŀ��λ��
            ref moveVelocity,          // ƽ���ٶ�
            pointSmoothtime                 // ƽ��ʱ��
        );

        transform.position = currentCameraPosition;
    }


    //������ת
    private void HandleRotations()
    {
        upAndDownLookAngle -= (playerManager.playerInputManager.cameraVerticalInput * upAndDownRotationSpeed) * Time.deltaTime;
        upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minimumPiovt, maximumPiovt);

        leftAndRightLookAngle += (playerManager.playerInputManager.cameraHorizontalInput * leftAndRightRotationSpeed) * Time.deltaTime;

        //�����ڷ�Χ��-180��180��
        if (leftAndRightLookAngle > 180)
        {
            leftAndRightLookAngle -= 360;
        }
        else if (leftAndRightLookAngle < -180f)
        {
            leftAndRightLookAngle += 360;
        }


        // ˮƽ��ת
        targetRotation = Quaternion.Euler(0f, leftAndRightLookAngle, 0f); //����һ����Ԫ������ʾ��y��תleftAndRightLookAngle��
        currentRotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothTime); //�������ǰ�ĽǶȣ���y����תleftAndRightLookAngle��
        transform.rotation = currentRotation;

        // ��ֱ��ת
        targetRotation = Quaternion.Euler(upAndDownLookAngle, 0f, 0f);
        cameraPivotTransform.localRotation = Quaternion.Slerp(cameraPivotTransform.localRotation, targetRotation, smoothTime);

    }
}
