using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static EnumManager;
using static EnemyAnimationName;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private string 当前姿态;

    [HideInInspector] public Animator animator;
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public CapsuleCollider capsuleCollider;

    [HideInInspector] public Transform player;


    public GameObject weapon;

    public EnemyAttackType attackType;
    public NavMeshAgent agent; //巡逻代理

    [Header("Locomotion")]
    public float walkSpeed;
    public float walkRotateSpeed;
    public float runSpeed;
    public float runRotateSpeed;

    public Transform guardPointParent; //巡逻点的父物体对象

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
        当前姿态 = currentState.ToString();

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

        // 应用位移
        if (rb != null && !rb.isKinematic)
        {
            // 使用Rigidbody移动和旋转（必须在FixedUpdate中才生效）
            rb.MovePosition(rb.position + rootMotionDelta);
            rb.MoveRotation(rb.rotation * rootRotationDelta);
        }
        else
        {
            // 如果是Kinematic Rigidbody 或者没加刚体，就直接修改Transform
            transform.position += rootMotionDelta;
            transform.rotation *= rootRotationDelta;
        }
    }*/

    #region 受击方法
    [Header("受击")]
    public bool isStoic; //是否处于霸体状态
    public float knockbackSpeedMul; //扩大动画曲线参数倍率
    public AnimationCurve damageLightBackMoveCurve; // 轻击后退速度曲线
    public AnimationCurve damageMediumBackMoveCurve; // 中击后退速度曲线
    public AnimationCurve damageHeavyBackMoveCurve; // 重击后退速度曲线

    /// <summary>
    /// 敌人受击逻辑
    /// </summary>
    /// <param name="_hitDirection">受击方向，由伤害来源指向自己</param>
    /// <param name="_damageType">受击类型</param>
    public void TakeDamage(Vector3 _hitDirection, EnemyDamageType _damageType = EnemyDamageType.None)
    {
        //处于霸体状态不会被打断动作
        if (isStoic) return;

        //根据受击类型选着不同的动画播放
        string animationName = "";
        switch (_damageType)
        {
            case EnemyDamageType.None:
                break;
            case EnemyDamageType.Light:
                // 根据受伤方向播放不同方向的受击动画
                float hitAngle = Vector3.SignedAngle(new Vector3(transform.forward.x, 0f, transform.forward.z), _hitDirection, Vector3.up);
                animationName = hitAngle switch
                {
                    >= -45f and <= 45f => Hit_B,
                    >= -135f and < -45f => Hit_L,
                    > 45f and <= 135f => Hit_R,
                    _ => Hit_F
                };
                // 切换为受击姿态
                currentState.ChangeState(hitState);

                hitState.damageType = _damageType;
                hitState.hitDir = _hitDirection;

                //进行动画过渡
                animator.CrossFadeInFixedTime(animationName, 0.1f);
                break;
            case EnemyDamageType.Medium:
                // 根据受伤方向播放不同方向的受击动画
                float hitAngle1 = Vector3.SignedAngle(new Vector3(transform.forward.x, 0f, transform.forward.z), _hitDirection, Vector3.up);
                animationName = hitAngle1 switch
                {
                    >= -45f and <= 45f => Hit_Large_B,
                    >= -135f and < -45f => Hit_Large_L,
                    > 45f and <= 135f => Hit_Large_R,
                    _ => Hit_Large_F
                };
                // 切换为受击姿态
                currentState.ChangeState(hitState);
                hitState.damageType = _damageType;
                hitState.hitDir = _hitDirection;
                //进行动画过渡
                animator.CrossFadeInFixedTime(animationName, 0.1f);
                break;
            case EnemyDamageType.Heavy:
                animationName = Hit_Down;
                // 切换为受击姿态
                currentState.ChangeState(hitState);
                hitState.damageType = _damageType;
                hitState.hitDir = _hitDirection;
                //进行动画过渡
                animator.CrossFadeInFixedTime(animationName, 0.1f);
                break;
            case EnemyDamageType.FlyAway:
                animationName = Hit_Air_Start;
                //注册施加击飞力的委托
                airHitState.action_addFlyForce += airHitState.AddFlyForceOnce;
                currentState.ChangeState(airHitState);
                airHitState.damageType = EnemyDamageType.FlyAway;
                animator.CrossFadeInFixedTime(animationName, 0.1f);
                break;
            case EnemyDamageType.AirLight:
                animationName = Hit_Air_Start;
                //注册空中受击力的委托
                airHitState.action_addAirForce += airHitState.AddAirForceOnce;
                currentState.ChangeState(airHitState);
                airHitState.damageType = EnemyDamageType.AirLight;
                animator.CrossFadeInFixedTime(animationName, 0.1f);
                break;
            default:
                Debug.LogError("出错了...");
                break;
        }
    }
    #endregion

    #region 判断是否处于可活动范围
    [Header("可活动最大范围")]
    public float maxChaseDistance; //与guardPointParent距离超过这个值时，放弃追逐，并回到guardPointParent位置
    //是否处于自身可活动范围：当锁定到玩家时，可以追逐的最大范围
    public bool IsInChaseRange()
    {
        return Vector3.Distance(guardPointParent.position, transform.position) < maxChaseDistance ? true : false;
    }
    #endregion

    #region 警戒玩家
    [Header("警戒玩家")]
    public float detectionRadius = 5f; // 检测半径5米
    public LayerMask playerLayer;     // 玩家所在层级
    public LayerMask obstacleLayer;   // 障碍物层级，必须包含Player
    public float fieldOfView = 135f;  // 视野角度范围
    public Vector3 detectionOffset; //相对于角色原点偏移量
    public bool GuardForPlayer()
    {
        // 球形检测范围内的所有碰撞体
        Collider[] hitColliders = Physics.OverlapSphere(transform.position + detectionOffset, detectionRadius, playerLayer);

        if (hitColliders.Length > 0)
        {
            // 假设场景中只有一个玩家
            player = hitColliders[0].transform;

            // 检查是否在视野内
            if (IsPlayerInFieldOfView(player))
            {
                return true;
            }
        }

        player = null;

        return false;
    }

    //辅助方法：检测玩家是否在视野范围内
    private bool IsPlayerInFieldOfView(Transform target)
    {
        // 计算敌人到玩家的方向向量
        Vector3 directionToTarget = (target.position - transform.position).normalized;

        // 计算与敌人正前方的夹角
        float angle = Vector3.Angle(transform.forward, directionToTarget);

        // 检查是否在视野角度范围内
        if (angle <= fieldOfView / 2f)
        {
            // 检查是否有障碍物阻挡
            RaycastHit hit;
            if (Physics.Raycast(transform.position + detectionOffset, directionToTarget, out hit, detectionRadius, obstacleLayer))
            {
                if (hit.transform == target) //说明检测到了player没有检测到障碍物
                {
                    return true;
                }
            }
        }
        return false;
    }
    #endregion

    #region 试探玩家

/*    当距离玩家比较近的距离内，则可以视为进入了可试探的范围；
    试探范围分为三个阶段：
    1.距离玩家非常近(贴近玩家)：此时试探时应当向后方移动(斜后方试探，或者直接后退试探)
    2.距离玩家有一小段距离：此时试探时应当围绕玩家进行左右试探
    3.距离玩家稍微有一段距离：此时试探应当向前方移动(斜前方试探)*/
    [Header("试探玩家")]
    public float exploreMaxDistance; //远距离
    public float exploreMedDistance; //中等距离
    public float exploreMinDistance; //近距离
    public float exploreRotateSpeed; //试探时的旋转速度
    public float exploreMoveSpeed; //试探时的移动速度

    //检测是否到达了可试探的范围
    public bool DetecteIsInExploreRange()
    {
        float dis = Vector3.Distance(transform.position, player.position); //获取与玩家的距离
        if (dis < exploreMaxDistance) return true;

        return false;
    }

    public EnemyExplorePlayerDistanceType DetecteExploreType()
    {
        float dis = Vector3.Distance(transform.position, player.position); //获取与玩家的距离

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

        return EnemyExplorePlayerDistanceType.None; //没有到达可试探的范围
    }

    #endregion

    #region 地面检测
    [HideInInspector] public Collider selfCollider;
    public float groundCheckRadius = 0.2f;
    public Vector3 groundCheckOffset = new Vector3(0, 0.1f, 0); // 从身体稍微往下

    public LayerMask groundLayer; // 在 Inspector 里指定只包含地面

    public bool IsGrounded()
    {
        Vector3 checkPosition = transform.position + groundCheckOffset;
        return Physics.CheckSphere(checkPosition, groundCheckRadius, groundLayer, QueryTriggerInteraction.Ignore);
    }



    #endregion



    private void OnDrawGizmosSelected()
    {
        #region 绘制地面检测范围
        Gizmos.color = Color.green;
        Vector3 checkPosition = transform.position + groundCheckOffset;
        Gizmos.DrawWireSphere(checkPosition, groundCheckRadius);
        #endregion

        #region 绘制试探范围
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, exploreMaxDistance);
        Gizmos.DrawWireSphere(transform.position, exploreMedDistance);
        Gizmos.DrawWireSphere(transform.position, exploreMinDistance);
        #endregion

        #region 绘制普通攻击范围
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + attackOffsetPoint, attackRadius);
        #endregion
    }

    private void OnDrawGizmos()
    {
        #region 绘制检测玩家的范围
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + detectionOffset, detectionRadius);

        // 绘制视野扇形
        if (player != null && IsPlayerInFieldOfView(player))
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position + detectionOffset, player.position);
        }

        // 绘制视野范围
        Gizmos.color = Color.cyan;
        float halfFOV = fieldOfView / 2f;
        Quaternion leftRayRotation = Quaternion.AngleAxis(-halfFOV, Vector3.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(halfFOV, Vector3.up);
        Vector3 leftRayDirection = leftRayRotation * transform.forward;
        Vector3 rightRayDirection = rightRayRotation * transform.forward;
        Gizmos.DrawRay(transform.position + detectionOffset, leftRayDirection * detectionRadius);
        Gizmos.DrawRay(transform.position + detectionOffset, rightRayDirection * detectionRadius);
        #endregion

        #region 绘制最大追逐范围
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(guardPointParent.transform.position,maxChaseDistance);
        #endregion


    }












    //动画事件

    #region 拔刀/收刀
    //拔刀
    public void Prepare()
    {
        weapon.SetActive(true);
    }
    //收刀
    public void Cancle()
    {
        weapon.SetActive(false);
    }
    #endregion


    #region 攻击检测
    [Header("攻击检测")]
    public float attackRadius; //攻击检测范围
    public Vector3 attackOffsetPoint; //攻击检测圆心与自身位置的偏移量
    public LayerMask attackTargetLayer; //可以攻击的对象层级
    //攻击检测
    public bool AttackDetecte(int _attackType)
    {
        // 将整数转换为 DamageType
        PlayerDamageType damageType = _attackType switch
        {
            0 => PlayerDamageType.None, 
            1 => PlayerDamageType.Light,  // 轻击
            2 => PlayerDamageType.Medium,   // 中等攻击
            3 => PlayerDamageType.Heavy,    // 重击
            4 => PlayerDamageType.FlyAway, //击飞
            5 => PlayerDamageType.AirLight, //空中轻击
            _ => PlayerDamageType.None, //默认无攻击类型
        };

        // 计算攻击偏移位置
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
                Debug.Log("空");
            }

            //获取面向玩家的方向向量
            Vector3 hitDir = playerManager.transform.position - transform.position;
            hitDir.y = 0;
            hitDir = hitDir.normalized;

            player.GetComponent<PlayerManager>().TakeDamage(hitDir, damageType);
        }

        return true;
    }
    #endregion

    #region 播放音效
    //随机播放音效
    #endregion
}