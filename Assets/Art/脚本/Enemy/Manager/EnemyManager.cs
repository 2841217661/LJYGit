using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static EnumManager;
using static EnemyAnimationName;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private string ��ǰ��̬;

    [HideInInspector] public Animator animator;
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public CapsuleCollider capsuleCollider;

    [HideInInspector] public Transform player;


    public GameObject weapon;

    public EnemyAttackType attackType;
    public NavMeshAgent agent; //Ѳ�ߴ���

    [Header("Locomotion")]
    public float walkSpeed;
    public float walkRotateSpeed;
    public float runSpeed;
    public float runRotateSpeed;

    public Transform guardPointParent; //Ѳ�ߵ�ĸ��������

    public EnemyState currentState;
    public EnemyIdleState idleState { get; private set; }
    public EnemyWalkState walkState { get; private set; }
    public EnemyChaseStartState chaseStartState { get; private set; }
    public EnemyRunState runState { get; private set; }
    public EnemyExploreState exploreState { get; private set; }
    public EnemyActionState actionState { get; private set; }
    public EnemyFallState fallState { get; private set; }
    public EnemyHitState hitState { get; private set; }
    public EnemyAirHitState airHitState { get; private set; }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        agent = GetComponent<NavMeshAgent>();
        selfCollider = GetComponent<Collider>();


        idleState = new EnemyIdleState(this, Idle, false);
        walkState = new EnemyWalkState(this, Walk, false);
        runState = new EnemyRunState(this, Run, false);
        chaseStartState = new EnemyChaseStartState(this, ChaseStart, false);
        exploreState = new EnemyExploreState(this, Explore, false); 
        actionState = new EnemyActionState(this, Action, true); 
        fallState = new EnemyFallState(this, Fall, false);
        hitState = new EnemyHitState(this, Hit, false);
        airHitState = new EnemyAirHitState(this, Hit,false);
    }

    private void Start()
    {
        currentState = idleState;

        currentState.ChangeState(idleState);

        attackType = EnemyAttackType.Normal;
    }

    private void Update()
    {
        currentState.Update();
        ��ǰ��̬ = currentState.ToString();

        if (Input.GetKeyDown(KeyCode.P))
        {
            TakeDamage(Vector3.up,EnemyDamageType.AirLight);
        }
    }

    private void FixedUpdate()
    {
        currentState.FixedUpdate();
    }

/*    private void OnAnimatorMove()
    {
        if (!currentState.useRootMotion)
            return;

        HandleRootMotion();
    }

    private void HandleRootMotion()
    {
        Vector3 rootMotionDelta = animator.deltaPosition;
        Quaternion rootRotationDelta = animator.deltaRotation;

        // Ӧ��λ��
        if (rb != null && !rb.isKinematic)
        {
            // ʹ��Rigidbody�ƶ�����ת��������FixedUpdate�в���Ч��
            rb.MovePosition(rb.position + rootMotionDelta);
            rb.MoveRotation(rb.rotation * rootRotationDelta);
        }
        else
        {
            // �����Kinematic Rigidbody ����û�Ӹ��壬��ֱ���޸�Transform
            transform.position += rootMotionDelta;
            transform.rotation *= rootRotationDelta;
        }
    }*/

    #region �ܻ�����
    [Header("�ܻ�")]
    public bool isStoic; //�Ƿ��ڰ���״̬
    public float knockbackSpeedMul; //���󶯻����߲�������
    public AnimationCurve damageLightBackMoveCurve; // ��������ٶ�����
    public AnimationCurve damageMediumBackMoveCurve; // �л������ٶ�����
    public AnimationCurve damageHeavyBackMoveCurve; // �ػ������ٶ�����

    /// <summary>
    /// �����ܻ��߼�
    /// </summary>
    /// <param name="_hitDirection">�ܻ��������˺���Դָ���Լ�</param>
    /// <param name="_damageType">�ܻ�����</param>
    public void TakeDamage(Vector3 _hitDirection, EnemyDamageType _damageType = EnemyDamageType.None)
    {
        //���ڰ���״̬���ᱻ��϶���
        if (isStoic) return;

        //�����ܻ�����ѡ�Ų�ͬ�Ķ�������
        string animationName = "";
        switch (_damageType)
        {
            case EnemyDamageType.None:
                break;
            case EnemyDamageType.Light:
                // �������˷��򲥷Ų�ͬ������ܻ�����
                float hitAngle = Vector3.SignedAngle(new Vector3(transform.forward.x, 0f, transform.forward.z), _hitDirection, Vector3.up);
                animationName = hitAngle switch
                {
                    >= -45f and <= 45f => Hit_B,
                    >= -135f and < -45f => Hit_L,
                    > 45f and <= 135f => Hit_R,
                    _ => Hit_F
                };
                // �л�Ϊ�ܻ���̬
                currentState.ChangeState(hitState);

                hitState.damageType = _damageType;
                hitState.hitDir = _hitDirection;

                //���ж�������
                animator.CrossFadeInFixedTime(animationName, 0.1f);
                break;
            case EnemyDamageType.Medium:
                // �������˷��򲥷Ų�ͬ������ܻ�����
                float hitAngle1 = Vector3.SignedAngle(new Vector3(transform.forward.x, 0f, transform.forward.z), _hitDirection, Vector3.up);
                animationName = hitAngle1 switch
                {
                    >= -45f and <= 45f => Hit_Large_B,
                    >= -135f and < -45f => Hit_Large_L,
                    > 45f and <= 135f => Hit_Large_R,
                    _ => Hit_Large_F
                };
                // �л�Ϊ�ܻ���̬
                currentState.ChangeState(hitState);
                hitState.damageType = _damageType;
                hitState.hitDir = _hitDirection;
                //���ж�������
                animator.CrossFadeInFixedTime(animationName, 0.1f);
                break;
            case EnemyDamageType.Heavy:
                animationName = Hit_Down;
                // �л�Ϊ�ܻ���̬
                currentState.ChangeState(hitState);
                hitState.damageType = _damageType;
                hitState.hitDir = _hitDirection;
                //���ж�������
                animator.CrossFadeInFixedTime(animationName, 0.1f);
                break;
            case EnemyDamageType.FlyAway:
                animationName = Hit_Air_Start;
                //ע��ʩ�ӻ�������ί��
                airHitState.action_addFlyForce += airHitState.AddFlyForceOnce;
                currentState.ChangeState(airHitState);
                airHitState.damageType = EnemyDamageType.FlyAway;
                animator.CrossFadeInFixedTime(animationName, 0.1f);
                break;
            case EnemyDamageType.AirLight:
                animationName = Hit_Air_Start;
                //ע������ܻ�����ί��
                airHitState.action_addAirForce += airHitState.AddAirForceOnce;
                currentState.ChangeState(airHitState);
                airHitState.damageType = EnemyDamageType.AirLight;
                animator.CrossFadeInFixedTime(animationName, 0.1f);
                break;
            default:
                Debug.LogError("������...");
                break;
        }
    }
    #endregion

    #region �ж��Ƿ��ڿɻ��Χ
    [Header("�ɻ���Χ")]
    public float maxChaseDistance; //��guardPointParent���볬�����ֵʱ������׷�𣬲��ص�guardPointParentλ��
    //�Ƿ�������ɻ��Χ�������������ʱ������׷������Χ
    public bool IsInChaseRange()
    {
        return Vector3.Distance(guardPointParent.position, transform.position) < maxChaseDistance ? true : false;
    }
    #endregion

    #region �������
    [Header("�������")]
    public float detectionRadius = 5f; // ���뾶5��
    public LayerMask playerLayer;     // ������ڲ㼶
    public LayerMask obstacleLayer;   // �ϰ���㼶���������Player
    public float fieldOfView = 135f;  // ��Ұ�Ƕȷ�Χ
    public Vector3 detectionOffset; //����ڽ�ɫԭ��ƫ����
    public bool GuardForPlayer()
    {
        // ���μ�ⷶΧ�ڵ�������ײ��
        Collider[] hitColliders = Physics.OverlapSphere(transform.position + detectionOffset, detectionRadius, playerLayer);

        if (hitColliders.Length > 0)
        {
            // ���賡����ֻ��һ�����
            player = hitColliders[0].transform;

            // ����Ƿ�����Ұ��
            if (IsPlayerInFieldOfView(player))
            {
                return true;
            }
        }

        player = null;

        return false;
    }

    //�����������������Ƿ�����Ұ��Χ��
    private bool IsPlayerInFieldOfView(Transform target)
    {
        // ������˵���ҵķ�������
        Vector3 directionToTarget = (target.position - transform.position).normalized;

        // �����������ǰ���ļн�
        float angle = Vector3.Angle(transform.forward, directionToTarget);

        // ����Ƿ�����Ұ�Ƕȷ�Χ��
        if (angle <= fieldOfView / 2f)
        {
            // ����Ƿ����ϰ����赲
            RaycastHit hit;
            if (Physics.Raycast(transform.position + detectionOffset, directionToTarget, out hit, detectionRadius, obstacleLayer))
            {
                if (hit.transform == target) //˵����⵽��playerû�м�⵽�ϰ���
                {
                    return true;
                }
            }
        }
        return false;
    }
    #endregion

    #region ��̽���

/*    ��������ұȽϽ��ľ����ڣ��������Ϊ�����˿���̽�ķ�Χ��
    ��̽��Χ��Ϊ�����׶Σ�
    1.������ҷǳ���(�������)����ʱ��̽ʱӦ������ƶ�(б����̽������ֱ�Ӻ�����̽)
    2.���������һС�ξ��룺��ʱ��̽ʱӦ��Χ����ҽ���������̽
    3.���������΢��һ�ξ��룺��ʱ��̽Ӧ����ǰ���ƶ�(бǰ����̽)*/
    [Header("��̽���")]
    public float exploreMaxDistance; //Զ����
    public float exploreMedDistance; //�еȾ���
    public float exploreMinDistance; //������
    public float exploreRotateSpeed; //��̽ʱ����ת�ٶ�
    public float exploreMoveSpeed; //��̽ʱ���ƶ��ٶ�

    //����Ƿ񵽴��˿���̽�ķ�Χ
    public bool DetecteIsInExploreRange()
    {
        float dis = Vector3.Distance(transform.position, player.position); //��ȡ����ҵľ���
        if (dis < exploreMaxDistance) return true;

        return false;
    }

    public EnemyExplorePlayerDistanceType DetecteExploreType()
    {
        float dis = Vector3.Distance(transform.position, player.position); //��ȡ����ҵľ���

        if (DetecteIsInExploreRange())
        {
            if(dis < exploreMedDistance && dis > exploreMinDistance)
            {
                return EnemyExplorePlayerDistanceType.Med;
            }
            else if(dis < exploreMinDistance)
            {
                return EnemyExplorePlayerDistanceType.Min;
            }
            else
            {
                return EnemyExplorePlayerDistanceType.Max;
            }
        }

        return EnemyExplorePlayerDistanceType.None; //û�е������̽�ķ�Χ
    }

    #endregion

    #region ������
    [HideInInspector] public Collider selfCollider;
    public float groundCheckRadius = 0.2f;
    public Vector3 groundCheckOffset = new Vector3(0, 0.1f, 0); // ��������΢����

    public LayerMask groundLayer; // �� Inspector ��ָ��ֻ��������

    public bool IsGrounded()
    {
        Vector3 checkPosition = transform.position + groundCheckOffset;
        return Physics.CheckSphere(checkPosition, groundCheckRadius, groundLayer, QueryTriggerInteraction.Ignore);
    }



    #endregion



    private void OnDrawGizmosSelected()
    {
        #region ���Ƶ����ⷶΧ
        Gizmos.color = Color.green;
        Vector3 checkPosition = transform.position + groundCheckOffset;
        Gizmos.DrawWireSphere(checkPosition, groundCheckRadius);
        #endregion

        #region ������̽��Χ
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, exploreMaxDistance);
        Gizmos.DrawWireSphere(transform.position, exploreMedDistance);
        Gizmos.DrawWireSphere(transform.position, exploreMinDistance);
        #endregion

        #region ������ͨ������Χ
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + attackOffsetPoint, attackRadius);
        #endregion
    }

    private void OnDrawGizmos()
    {
        #region ���Ƽ����ҵķ�Χ
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + detectionOffset, detectionRadius);

        // ������Ұ����
        if (player != null && IsPlayerInFieldOfView(player))
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position + detectionOffset, player.position);
        }

        // ������Ұ��Χ
        Gizmos.color = Color.cyan;
        float halfFOV = fieldOfView / 2f;
        Quaternion leftRayRotation = Quaternion.AngleAxis(-halfFOV, Vector3.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(halfFOV, Vector3.up);
        Vector3 leftRayDirection = leftRayRotation * transform.forward;
        Vector3 rightRayDirection = rightRayRotation * transform.forward;
        Gizmos.DrawRay(transform.position + detectionOffset, leftRayDirection * detectionRadius);
        Gizmos.DrawRay(transform.position + detectionOffset, rightRayDirection * detectionRadius);
        #endregion

        #region �������׷��Χ
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(guardPointParent.transform.position,maxChaseDistance);
        #endregion


    }












    //�����¼�

    #region �ε�/�յ�
    //�ε�
    public void Prepare()
    {
        weapon.SetActive(true);
    }
    //�յ�
    public void Cancle()
    {
        weapon.SetActive(false);
    }
    #endregion


    #region �������
    [Header("�������")]
    public float attackRadius; //������ⷶΧ
    public Vector3 attackOffsetPoint; //�������Բ��������λ�õ�ƫ����
    public LayerMask attackTargetLayer; //���Թ����Ķ���㼶
    //�������
    public bool AttackDetecte(int _attackType)
    {
        // ������ת��Ϊ DamageType
        PlayerDamageType damageType = _attackType switch
        {
            0 => PlayerDamageType.None, 
            1 => PlayerDamageType.Light,  // ���
            2 => PlayerDamageType.Medium,   // �еȹ���
            3 => PlayerDamageType.Heavy,    // �ػ�
            4 => PlayerDamageType.FlyAway, //����
            5 => PlayerDamageType.AirLight, //�������
            _ => PlayerDamageType.None, //Ĭ���޹�������
        };

        // ���㹥��ƫ��λ��
        Vector3 offsetPoint = transform.rotation * attackOffsetPoint;

        Collider[] players = Physics.OverlapSphere(transform.position + offsetPoint, attackRadius, attackTargetLayer);

        if (players.Length == 0)
        {
            return false;
        }

        foreach (Collider player in players)
        {
            PlayerManager playerManager = player.GetComponent<PlayerManager>();
            if (playerManager == null)
            {
                Debug.Log("��");
            }

            //��ȡ������ҵķ�������
            Vector3 hitDir = playerManager.transform.position - transform.position;
            hitDir.y = 0;
            hitDir = hitDir.normalized;

            player.GetComponent<PlayerManager>().TakeDamage(hitDir, damageType);
        }

        return true;
    }
    #endregion

    #region ������Ч
    //���������Ч
    #endregion
}