using UnityEngine;

public class PlayerPowerAttackCamera : MonoBehaviour
{
    private PlayerManager playerManager; // ��ҹ����������ڻ�ȡ��ҵ�λ������ת

    [Header("Camera Settings")]
    private Vector3 localOffset = new Vector3(1.12f, -0.39f, -0.48f);
    // �������ұ�������ռ��е�ƫ��������������������ҵ�λ�ã�����λ����󷽡���΢��һЩ

    private Vector3 rotationOffset = new Vector3(-11.9f, 120f, 0f);
    // ����������ҵ�ǰ�������תƫ�ƣ�����Ӳ�󷽸��ӽ�ɫ����

    private Transform cameraPivotTransform; // ������м���ת��Ŧ�����ڴ���������ת��

    private Vector3 endPosition;
    private Quaternion endRotation;

    [Header("Smoothing Curve")]
    private AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    // Ĭ�Ͼ����������������EaseInOut��
    private float transitionDuration = 7f; // �������ɹ��̳���ʱ��
    private float transitionTimer = 0f;


    private void Awake()
    {
        // ��ȫ������������л�ȡ�����Ŧ��cameraPivotTransform��������
        cameraPivotTransform = CameraManager.Instance.playerCameraManager.playerCameraPivotTransform;

        playerManager = CameraManager.Instance.playerCameraManager.playerManager;
    }

    private void OnEnable()
    {
        // ���������Ŧ�ı�����ת����ֹ�ɵ��ӽǻ����Ӱ�쵱ǰ��ͷ�Ƕ�
        cameraPivotTransform.localRotation = Quaternion.identity;

        // �������Ŀ����������λ�ã�
        // ������ƫ���� localOffset Ӧ�õ���ҵ�����������
        Vector3 targetPosition = playerManager.transform.position +
                                 playerManager.transform.rotation * localOffset;

        // �������Ŀ��������ת�Ƕȣ�
        // ������ı�����ת offset����������Ҳ��� 120 �ȣ�Ӧ�õ���ҵĵ�ǰ��ת��
        Quaternion targetRotation = playerManager.transform.rotation * Quaternion.Euler(rotationOffset);

        // ���������λ�úͽǶȣ�ʹ���ָ���Ƕȿ������
        transform.position = targetPosition;
        transform.rotation = targetRotation;

        endPosition = playerManager.transform.position;
        endRotation = playerManager.transform.rotation;

        // ���������ʼ״̬
        transitionTimer = 0f;
    }

    private void LateUpdate()
    {
        transitionTimer += Time.deltaTime;
        float t = Mathf.Clamp01(transitionTimer / transitionDuration);
        float curvedT = transitionCurve.Evaluate(t); // ���߿��Ʋ�ֵ�ٶ�

        transform.position = Vector3.Lerp(transform.position, endPosition, curvedT);
        transform.rotation = Quaternion.Slerp(transform.rotation, endRotation, curvedT);
    }

}
