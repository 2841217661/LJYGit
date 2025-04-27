using System.Collections;
using UnityEngine;

public class PlayerNormalCamera : MonoBehaviour
{
    public PlayerManager playerManager;


    private Transform cameraTransform; //�����
    private Transform cameraPivotTransform; //���ظýű��Ķ�����������ת�������帺��������ת���������ֵ����ײ(z�ķ�Χ����)

    private Vector3 pivotOffset = new Vector3(0, 1.5f, 0); // cameraPivotTransform��ƫ��
    private Vector3 cameraOffset = new Vector3(0, 0, -3); // �������ƫ��

    [Header("Camera Setting")]
    private float cameraSmoothSpeed = 0.05f; //����ƶ�ƽ���ٶ�
    private float leftAndRightRotationSpeed = 200; //������ת�ٶ�
    private float upAndDownRotationSpeed = 200; //������ת�ٶ�
    private float minimumPiovt = -30; //���������ת�Ƕ�
    private float maximumPiovt = 60; //���������ת�Ƕ�
    private float cameraCollisionRadius = 0.2f; //Բ����ײ���뾶
    [SerializeField] private LayerMask collideWithLayers; //�������ײ�㼶
    private float smoothTime = 0.1f; // ����ƽ��ʱ��
    private Quaternion currentRotation;
    private Quaternion targetRotation;

    [Header("Camera Values")]
    private Vector3 cameraVelocity; //�������ʱ��ƽ������ο�ֵ
    private Vector3 cameraObjectPosition; //����ײ����Ӵ�ʱ����Ҫ����λ�ã����ֵ�����洢��Ӧֵ
    private float leftAndRightLookAngle; //������ת�ĽǶ�
    private float upAndDownLookAngle; //������ת�ĽǶ�
    private float cameraZPosition; //��ײʱ�õ�
    private float targetCameraZPosition; //��ײʱ�õ�

    private void Awake()
    {
        cameraPivotTransform = CameraManager.Instance.playerCameraManager.playerCameraPivotTransform;

        cameraTransform = CameraManager.Instance.playerCameraManager.playerMainCamera.transform;

        //������������player��λ��
        cameraPivotTransform.localPosition = pivotOffset;
        cameraTransform.transform.localPosition = cameraOffset;
    }

    private void OnEnable()
    {
        leftAndRightLookAngle = transform.rotation.eulerAngles.y;
        // �������½Ƕȵĳ�ʼ������ 0~360 תΪ -180~180��
        float rawUpDownAngle = cameraPivotTransform.localRotation.eulerAngles.x;
        upAndDownLookAngle = (rawUpDownAngle > 180f) ? rawUpDownAngle - 360f : rawUpDownAngle;
        cameraVelocity = Vector3.zero;
    }
    private void Start()
    {
        cameraZPosition = cameraTransform.transform.localPosition.z; //��ʼʱ��¼�����z�������һʱ�䷢����ײ����ô�ͻ���ж�Ӧ����ײ����
    }


    private void LateUpdate()
    {
        HandleAllCameraActions();
    }

    /// <summary>
    /// �������������Ϊ
    /// </summary>
    public void HandleAllCameraActions()
    {
        HandleFollowTarget();
        HandleRotations();
        HandleCollision();
    }

    /// <summary>
    /// ���洦����̬����ƽ���ٶȣ�
    /// </summary>
    public void HandleFollowTarget()
    {
        // ��̬���� cameraSmoothSpeed ���� yVelocity
        float minSpeed = 0.03f;
        float maxSpeed = 0.08f;
        float maxVelocityThreshold = 20f;
        float safeVelocityThreshold = 10f;

        // ����ƽ���ٶȣ����� yVelocity �ľ���ֵ��
        float absYVelocity = Mathf.Abs(playerManager.yVelocity.y);
        if (absYVelocity <= safeVelocityThreshold)
        {
            // yVelocity �� [-10, 10] �� cameraSmoothSpeed = 0.08
            cameraSmoothSpeed = maxSpeed;
        }
        else if (absYVelocity >= maxVelocityThreshold)
        {
            // yVelocity �� -20 �� �� 20 �� cameraSmoothSpeed = 0.01
            cameraSmoothSpeed = minSpeed;
        }
        else
        {
            // yVelocity �� (-20, -10) �� (10, 20) �� ���Բ�ֵ
            float t = Mathf.InverseLerp(safeVelocityThreshold, maxVelocityThreshold, absYVelocity);
            cameraSmoothSpeed = Mathf.Lerp(maxSpeed, minSpeed, t);
        }

        // ƽ�������ɫλ��
        Vector3 targetCameraPosition = Vector3.SmoothDamp(
            transform.position,
            playerManager.transform.position,
            ref cameraVelocity,
            cameraSmoothSpeed
        );
        transform.position = targetCameraPosition;
    }

    /// <summary>
    /// ��ת����
    /// </summary>
    public void HandleRotations()
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

    /// <summary>
    /// ��ײ����
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

    //������"
    private float shakeFrequency = 25f; // ���� Perlin ��������Ƶ�ʣ�ֵԽ�߱仯Խ��
    private float shakeRotationFactor = 0.1f;// ������ת���ȣ������λ�÷��ȵĳ�����

    private bool isShaking = false;
    private Vector3 originalCameraPosition; // ��ʼ�ֲ�λ��
    private Quaternion originalCameraRotation; // ��ʼ�ֲ���ת
    private Vector3 noiseSeed; // ���� Perlin �������������

    /// <summary>
    /// ����һ�������
    /// </summary>
    /// <param name="duration">�𶯳���ʱ��</param>
    /// <param name="magnitude">����𶯷���</param>
    public void StartCameraShake(float duration, float magnitude)
    {
        if (isShaking) return;

        // ��¼��ʼλ������ת
        originalCameraPosition = cameraTransform.localPosition;
        originalCameraRotation = cameraTransform.localRotation;

        // ���������������������
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

            // ����˥�����ߣ����滻Ϊ�������ߣ�
            float attenuation = Mathf.Pow(1 - progress, 3);
            float currentMagnitude = initialMagnitude * attenuation;

            // ��άPerlin���� ��������� Perlin ����ֵ [-1, 1]
            float x = (Mathf.PerlinNoise(noiseSeed.x, Time.time * shakeFrequency) * 2 - 1);
            float y = (Mathf.PerlinNoise(noiseSeed.y, Time.time * shakeFrequency) * 2 - 1);
            float z = (Mathf.PerlinNoise(noiseSeed.z, Time.time * shakeFrequency) * 2 - 1);

            // Ӧ��λ��ƫ��
            Vector3 positionOffset = new Vector3(x, y, z) * currentMagnitude;
            cameraTransform.localPosition = originalCameraPosition + positionOffset;

            // Ӧ����תƫ�ƣ���ѡ��
            if (shakeRotationFactor > 0)
            {
                Vector3 rotationOffset = new Vector3(y, x, z) * currentMagnitude * shakeRotationFactor;
                cameraTransform.localRotation = originalCameraRotation * Quaternion.Euler(rotationOffset);
            }

            yield return null;
        }

        // ��׼��ԭ��ʼ״̬
        cameraTransform.localPosition = originalCameraPosition;
        cameraTransform.localRotation = originalCameraRotation;
        isShaking = false;
    }

}