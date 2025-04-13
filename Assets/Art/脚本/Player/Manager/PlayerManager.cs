using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    [HideInInspector] public Animator animator;
    [HideInInspector] public PlayerInputManager playerInputManager;
    [HideInInspector] public CharacterController characterController;

    [SerializeField] private string 当前姿态;
    public float a;
    public float b;
    [Header("Walk")]
    public float walkRotationSpeed;
    public float walkSpeed;

    [Header("Run")]
    public float runRotationSpeed;
    public float runSpeed;
    public float runStopSpeed;

    [Header("Rotate")]
    public float turnAngleMul;

    [Header("Dodge")]
    public float dodgeRotateSpeed;

    [Header("Sprint")]
    public float sprintRotationSpeed;
    public float sprintSpeed;

    [Header("地面检测")]
    public bool isGrounded;
    public LayerMask groundLayer; //检测层级
    public float groundCheckSphereRadius; //使用球型检测，检测的半径,与characterControll半径一致最为合适


    [Header("跳跃")]
    public Vector3 yVelocity; //垂直速度
    public float groundedYVelocity = -2f; //在地面上的垂直速度,使角色贴紧地面
    public float gravity; //重力加速度
    public float jumpHeight; // 跳跃高度
    public float jumpRotationSpeed; //跳跃姿态旋转速度
    public float jumpWalkMoveSpeed; //跳跃姿态移动速度
    public float jumpRunMoveSpeed;
    public float jumpSprintMoveSpeed;
    public float inAirTimer; //下落在空中的时间
    public bool useGravity = true;

    [Header("攻击范围敌人锁定")]
    public float attackDetectionRadius; //检测半径
    public LayerMask attackDetectionLayer; //检测层级，一般为Enemy

    [Header("攻击范围")]
    public float attackRadius; //攻击半径
    public Vector3 attackOffsetPoint; //相对于自身位置偏移量
    public LayerMask attackTargetLayer; //攻击对象层级，一般为Enemy
    public float attackTouchRadius;

    private EnumManager.PlayerAttackState _playerAttack = EnumManager.PlayerAttackState.normal;
    public EnumManager.PlayerAttackState playerAttack
    {
        get { return _playerAttack; }
        set
        {
            if (_playerAttack != value)  // 只有当值真正变化时才触发
            {
                _playerAttack = value;
                animator.SetFloat("AttackState", (float)value);
            }
        }
    }

    [Header("Combo")]
    public List<PlayerComboConfi> comboConfi;
    public float comboRotateSpeed; //连击中的旋转速度

    #region 申明状态

    public PlayerState currentState;
    public PlayerIdleState idleState { get; private set; }
    public PlayerWalkState walkState { get; private set; }

    public PlayerWalkStopState walkStopState { get; private set; }
    public PlayerRunState runState { get; private set; }
    public PlayerRunStopState runStopState { get; private set; }
    public PlayerDodgeState dodgeState { get; private set; }
    public PlayerSprintState sprintState { get; private set; }
    public PlayerSprintStopState sprintStopState { get; private set; }
    public PlayerJumpStartState jumpStartState { get; private set; }
    public PlayerJumpLoopState jumpLoopState { get; private set; }
    public PlayerJumpStopState jumpStopState { get; private set; }
    public PlayerCombo_1_1State combo_1_1State { get; private set; }
    public PlayerCombo_1_2State combo_1_2State { get; private set; }
    public PlayerCombo_1_3State combo_1_3State { get; private set; }
    public PlayerCombo_1_4State combo_1_4State { get; private set; }
    public PlayerCombo_1_5State combo_1_5State { get; private set; }
    public PlayerSprintAttackState sprintAttackState { get; private set; }
    public PlayerChargeAttack_1State chargeAttack_1State { get; private set; }
    public PlayerAirIdleState airIdleState { get; private set; }
    public PlayerAirDodgeState airDodgeState { get; private set; }
    public PlayerAirCombo_1_1State airCombo_1_1State { get; private set; }
    public PlayerAirCombo_1_2State airCombo_1_2State { get; private set; }
    public PlayerAirCombo_1_3State airCombo_1_3State { get; private set; }
    public PlayerAirCombo_1_4State airCombo_1_4State { get; private set; }
    public PlayerAirChargeAttack_1StartState airChargeAttack_1StartState { get; private set; }
    public PlayerAirChargeAttack_1LoopState airChargeAttack_1LoopState { get; private set; }
    public PlayerAirChargeAttack_1EndState airChargeAttack_1EndState { get; private set; }
    public PlayerAirChargeAttack_2StartState airChargeAttack_2StartState { get; private set; }
    public PlayerAirChargeAttack_2LoopState airChargeAttack_2LoopState { get; private set; }
    public PlayerAirChargeAttack_2EndState airChargeAttack_2EndState { get; private set; }
    public PlayerPowerAttackStartState powerAttackStartState { get; private set; }
    public PlayerPowerAttackLoopState powerAttackLoopState { get; private set; }
    public PlayerPowerAttackEndState powerAttackEndState { get; private set; }

    #endregion

    private void Awake()
    {

        animator = GetComponent<Animator>();
        playerInputManager = GetComponent<PlayerInputManager>();
        characterController = GetComponent<CharacterController>();

        //初始化状态
        idleState = new PlayerIdleState(this, PlayerAnimationName.Idle, false);
        walkState = new PlayerWalkState(this, PlayerAnimationName.Walk, false);
        walkStopState = new PlayerWalkStopState(this, PlayerAnimationName.WalkStop,false);
        runState = new PlayerRunState(this, PlayerAnimationName.Run, false);
        runStopState = new PlayerRunStopState(this, PlayerAnimationName.RunStop,false );
        dodgeState = new PlayerDodgeState(this, PlayerAnimationName.Dodge, true);
        sprintState = new PlayerSprintState(this, PlayerAnimationName.Sprint, false);
        sprintStopState = new PlayerSprintStopState(this, PlayerAnimationName.SprintStop, false);
        jumpStartState = new PlayerJumpStartState(this, PlayerAnimationName.JumpStart, false);
        jumpLoopState = new PlayerJumpLoopState(this, PlayerAnimationName.JumpLoop, false);
        jumpStopState = new PlayerJumpStopState(this, PlayerAnimationName.JumpStop,false);
        combo_1_1State = new PlayerCombo_1_1State(this, PlayerAnimationName.Combo_1_1,true);
        combo_1_2State = new PlayerCombo_1_2State(this, PlayerAnimationName.Combo_1_2, true);
        combo_1_3State = new PlayerCombo_1_3State(this, PlayerAnimationName.Combo_1_3, true);
        combo_1_4State = new PlayerCombo_1_4State(this, PlayerAnimationName.Combo_1_4, true);
        combo_1_5State = new PlayerCombo_1_5State(this, PlayerAnimationName.Combo_1_5, true);
        airCombo_1_1State = new PlayerAirCombo_1_1State(this, PlayerAnimationName.AirCombo_1_1, true);
        airCombo_1_2State = new PlayerAirCombo_1_2State(this, PlayerAnimationName.AirCombo_1_2, true);
        airCombo_1_3State = new PlayerAirCombo_1_3State(this, PlayerAnimationName.AirCombo_1_3, true);
        airCombo_1_4State = new PlayerAirCombo_1_4State(this, PlayerAnimationName.AirCombo_1_4, true);
        sprintAttackState = new PlayerSprintAttackState(this,PlayerAnimationName.SprintAttack,true);
        chargeAttack_1State = new PlayerChargeAttack_1State(this, PlayerAnimationName.ChargeAttack_1,true);
        airIdleState = new PlayerAirIdleState(this,PlayerAnimationName.AirIdle, false);
        airDodgeState = new PlayerAirDodgeState(this,PlayerAnimationName.AirDodge,true);
        airChargeAttack_1StartState = new PlayerAirChargeAttack_1StartState(this, "",false);
        airChargeAttack_1LoopState = new PlayerAirChargeAttack_1LoopState(this, "",false);
        airChargeAttack_1EndState = new PlayerAirChargeAttack_1EndState(this, "",false);
        airChargeAttack_2StartState = new PlayerAirChargeAttack_2StartState(this, PlayerAnimationName.AirChargeAttack_2Start, false);
        airChargeAttack_2LoopState = new PlayerAirChargeAttack_2LoopState(this, PlayerAnimationName.AirChargeAttack_2Loop, false);
        airChargeAttack_2EndState = new PlayerAirChargeAttack_2EndState(this, PlayerAnimationName.AirChargeAttack_2End, false);
        powerAttackStartState = new PlayerPowerAttackStartState(this, PlayerAnimationName.PowerAttackStart, true);
        powerAttackLoopState = new PlayerPowerAttackLoopState(this, PlayerAnimationName.PowerAttackLoop, true);
        powerAttackEndState = new PlayerPowerAttackEndState(this, PlayerAnimationName.PowerAttackEnd, true);
    }

    private void Start()
    {
        currentState = idleState; //idle为默认姿态
    }

    private void Update()
    {
        //输入获取
        playerInputManager.GetAllInput();

        //持续调用当前姿态的Update方法
        currentState.Update();

        当前姿态 = currentState.ToString();


        //测试
        if (Input.GetKeyDown(KeyCode.R))
        {
            CameraManager.Instance.playerCameraManager.playerNormalCamera.StartCameraShake(1f, 3f);
        }
    }

    private void OnDrawGizmos()
    {
        // 绘制攻击锁定敌人检测范围
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackDetectionRadius);

        //绘制攻击范围
        Gizmos.color = Color.red;
        //Vector3 offsetPoint = transform.TransformDirection(attackOffsetPoint); //角色的缩放会对偏移量产生影响
        Vector3 offsetPoint = transform.rotation * attackOffsetPoint;
        Gizmos.DrawWireSphere(transform.position + offsetPoint, attackRadius);

        //绘制触碰敌人检测范围
        // 获取CharacterController的组件
        CharacterController characterController = GetComponent<CharacterController>();

        // 获取CharacterController的碰撞器中心
        Vector3 colliderCenter = characterController.bounds.center;

        // 计算球形检测的半径
        float detectionRadius = characterController.radius + attackTouchRadius;

        // 绘制检测范围（球体）
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(colliderCenter, detectionRadius);
    }

    private void OnDrawGizmosSelected()
    {
        // 获取角色控制器组件
        characterController = GetComponent<CharacterController>();

        // 绘制地面检测范围
        Gizmos.color = Color.gray;
        Vector3 lowestPoint = new Vector3(
            transform.position.x,
            characterController.bounds.min.y + groundCheckSphereRadius - 0.05f,
            transform.position.z
        );
        Gizmos.DrawSphere(lowestPoint, groundCheckSphereRadius); // 实心球
    }

    /// <summary>
    /// 应用重力
    /// </summary>
    public void HandleGravity(bool _user = true)
    {
        if (!_user) return;

        isGrounded = GroundCheck();

        //如果在地面上;有两种情况，第一种：从高出下落到地面，此时的判断条件为isGrounded为true并且yVelocity.y < 0;
        //                         第二种：当从地面跳跃时，刚开始虽然角色离开了地面，但是任然地面检测到了地面
        if (isGrounded && yVelocity.y < 0f) //下落到地面
        {
            yVelocity.y = groundedYVelocity;
            inAirTimer = 0f;
        }
        else //下落中,下落时的重力加速度会大一些，不然下落的时候感觉会轻飘飘的
        {
            if (yVelocity.y < 0f)
            {
                yVelocity.y += gravity * 1.5f * Time.deltaTime;
                //yVelocity.y += gravity * Time.deltaTime;
            }
            else
            {
                yVelocity.y += gravity * Time.deltaTime;
            }

            inAirTimer += Time.deltaTime;

            //限制y的速度，如果太大了会造成相机抖动
            yVelocity.y = Mathf.Clamp(yVelocity.y, -20f, 50f);

            //垂直方向移动
        }

        if (useGravity)
        {
            characterController.Move(yVelocity * Time.deltaTime);
        }
    }

    private void OnAnimatorMove()
    {
        // 如果不使用Root Motion，直接返回
        if (!currentState.useRootMotion)
        {
            return;
        }

        // 使用Root Motion
        HandleRootMotion(); 
    }

    private void HandleRootMotion()
    {
        // 获取Animator的位移和旋转数据
        Vector3 rootMotionDelta = animator.deltaPosition; // 位移增量
        Quaternion rootRotationDelta = animator.deltaRotation; // 旋转增量

        // 应用位移
        if (characterController.enabled)
        {
            // 使用CharacterController移动角色
            characterController.Move(rootMotionDelta);
        }

        // 应用旋转
        transform.rotation *= rootRotationDelta;
    }

    //地面检测
    public bool GroundCheck()
    {
        Vector3 lowestPoint = new Vector3(
            transform.position.x,
            characterController.bounds.min.y + groundCheckSphereRadius - 0.05f, // 上移一点
            transform.position.z
        );

        return Physics.CheckSphere(lowestPoint, groundCheckSphereRadius, groundLayer);
    }

    //攻击时周围范围敌人锁定检测
    public Vector3 DetectionEnemyPositionInAttack()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, attackDetectionRadius, attackDetectionLayer);

        //检测到敌人
        if(enemies.Length > 0)
        {
            //获取距离最近的enemy位置
            Transform nearestEnemy = null;
            float closestDistance = Mathf.Infinity;

            foreach (Collider col in enemies)
            {
                float distance = Vector3.Distance(transform.position, col.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    nearestEnemy = col.transform;
                }
            }
            return nearestEnemy.position;
        }
        return Vector3.zero;
    }

    //攻击姿态(如果攻击到敌人)限制rootMotion移动
    public bool AttackTouchEnemyDetecte()
    {
        // 获取CharacterController的碰撞器中心
        Vector3 colliderCenter = characterController.bounds.center;

        // 计算球形检测的半径
        float detectionRadius = characterController.radius + attackTouchRadius;

        // 使用OverlapSphere检测指定半径范围内的碰撞体
        Collider[] colliders = Physics.OverlapSphere(colliderCenter, detectionRadius, attackDetectionLayer);

        if(colliders.Length == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }


    #region 动画事件

    //攻击检测
    public void AttackDetecte(int _attackType)
    {
        // 将整数转换为 DamageType
        EnemyManager.DamageType damageType = _attackType switch
        {
            0 => EnemyManager.DamageType.None,
            1 => EnemyManager.DamageType.Light,  // 轻击
            2 => EnemyManager.DamageType.Medium,   // 中等攻击
            3 => EnemyManager.DamageType.Heavy,    // 重击
            _ => EnemyManager.DamageType.None, //默认无攻击类型
        };

        // 计算攻击偏移位置
        Vector3 offsetPoint = transform.rotation * attackOffsetPoint;

        Collider[] enemys = Physics.OverlapSphere(transform.position + offsetPoint, attackRadius, attackTargetLayer);

        if (enemys.Length == 0)
        {
            return;
        }

        //攻击到敌人概率抖动相机
        float chance = Random.Range(0, 1f);
        if (chance > 0.5f)
        {
            CameraManager.Instance.playerCameraManager.playerNormalCamera.StartCameraShake(0.5f, 0.3f);
        }

        foreach (Collider enemy in enemys)
        {
            EnemyManager enemyManager = enemy.GetComponent<EnemyManager>();
            if (enemyManager == null)
            {
                Debug.Log("空");
            }

            //获取面向敌人的方向向量
            Vector3 hitDir = enemyManager.transform.position - transform.position;
            hitDir.y = 0;
            hitDir = hitDir.normalized;

            enemy.GetComponent<EnemyManager>().TakeDamage(hitDir, damageType);
        }
    }

    //播放音效

    #endregion
}
