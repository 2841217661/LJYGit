                                                                                                                                                                                      /* 
                                            ╱|、喵喵神保佑
                                            (°､ 。 7  永无BUG
                                            |、ヽ  〳      
                                            ＿／  ／  ノ  ヽ
                                            ／　 ／＞⁰－。─ ミ＿xノ 
                                            /　/ ／　  ／ ／ ／  |
                                            /　/ /    ／ ／ ／   ヽ
                                            ﾉ　 │     /  / ／     | |
                                            ∠二二二二二二二二二二二二二ヽ
                                            |　　　　　  Code by 猫猫教信徒　　　　　　|
                                            |　　　　　  2025 喵历·吉祥之年　　　　　|
                                            ＼　                                  ／
                                            ──══━ ฅ•ω•ฅ 🐾 猫力护体 🐾ฅ•ω•ฅ ━══──
                                            ✧･ﾟ: *✧･ﾟ:* 魔法结界 *:･ﾟ✧*:･ﾟ✧

                                            ✦ 编译必过 ✦  
                                            ✦ 运行如风 ✦  
                                            ✦ 零 warning ✦  
                                            ✦ 内存安全 ✦  

                                            ♡〜٩(⸝⸝⸝◕ั ॣ ◕ั⸝⸝⸝)۶〜♡ 猫爪赐福 ♡〜٩(ˊ◡ˋ )۶〜♡
                                                                                                                                                                                      */



using System.Collections.Generic;
using UnityEngine;
using static EnumManager;
using static PlayerAnimationName;

public class PlayerManager : MonoBehaviour
{

    [HideInInspector] public Animator animator;
    [HideInInspector] public PlayerInputManager playerInputManager;
    [HideInInspector] public CharacterController characterController;

    [SerializeField] private string 当前姿态;

    public Camera playerCamera;
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



    [Header("跳跃")]

    public float jumpHeight; // 跳跃高度
    public float jumpRotationSpeed; //跳跃姿态旋转速度
    public float jumpWalkMoveSpeed; //跳跃姿态移动速度
    public float jumpRunMoveSpeed;
    public float jumpSprintMoveSpeed;
    public float inAirTimer; //下落在空中的时间






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
    public PlayerGroundHitState groundHitState { get; private set; }

    #endregion

    private void Awake()
    {

        animator = GetComponent<Animator>();
        playerInputManager = GetComponent<PlayerInputManager>();
        characterController = GetComponent<CharacterController>();

        #region 初始化状态
        idleState = new PlayerIdleState(this, Idle, false);
        walkState = new PlayerWalkState(this, Walk, false);
        walkStopState = new PlayerWalkStopState(this, WalkStop, false);
        runState = new PlayerRunState(this, Run, false);
        runStopState = new PlayerRunStopState(this, RunStop, false);
        dodgeState = new PlayerDodgeState(this, Dodge, true);
        sprintState = new PlayerSprintState(this, Sprint, false);
        sprintStopState = new PlayerSprintStopState(this, SprintStop, false);
        jumpStartState = new PlayerJumpStartState(this, JumpStart, false);
        jumpLoopState = new PlayerJumpLoopState(this, JumpLoop, false);
        jumpStopState = new PlayerJumpStopState(this, JumpStop, false);
        combo_1_1State = new PlayerCombo_1_1State(this, Combo_1_1, true);
        combo_1_2State = new PlayerCombo_1_2State(this, Combo_1_2, true);
        combo_1_3State = new PlayerCombo_1_3State(this, Combo_1_3, true);
        combo_1_4State = new PlayerCombo_1_4State(this, Combo_1_4, true);
        combo_1_5State = new PlayerCombo_1_5State(this, Combo_1_5, true);
        airCombo_1_1State = new PlayerAirCombo_1_1State(this, AirCombo_1_1, true);
        airCombo_1_2State = new PlayerAirCombo_1_2State(this, AirCombo_1_2, true);
        airCombo_1_3State = new PlayerAirCombo_1_3State(this, AirCombo_1_3, true);
        airCombo_1_4State = new PlayerAirCombo_1_4State(this, AirCombo_1_4, true);
        sprintAttackState = new PlayerSprintAttackState(this, SprintAttack, true);
        chargeAttack_1State = new PlayerChargeAttack_1State(this, ChargeAttack_1, true);
        airIdleState = new PlayerAirIdleState(this, AirIdle, false);
        airDodgeState = new PlayerAirDodgeState(this, AirDodge, true);
        airChargeAttack_1StartState = new PlayerAirChargeAttack_1StartState(this, "", false);
        airChargeAttack_1LoopState = new PlayerAirChargeAttack_1LoopState(this, "", false);
        airChargeAttack_1EndState = new PlayerAirChargeAttack_1EndState(this, "", false);
        airChargeAttack_2StartState = new PlayerAirChargeAttack_2StartState(this, AirChargeAttack_2Start, false);
        airChargeAttack_2LoopState = new PlayerAirChargeAttack_2LoopState(this, AirChargeAttack_2Loop, false);
        airChargeAttack_2EndState = new PlayerAirChargeAttack_2EndState(this, AirChargeAttack_2End, false);
        powerAttackStartState = new PlayerPowerAttackStartState(this, PowerAttackStart, true);
        powerAttackLoopState = new PlayerPowerAttackLoopState(this, PowerAttackLoop, true);
        powerAttackEndState = new PlayerPowerAttackEndState(this, PowerAttackEnd, true);
        groundHitState = new PlayerGroundHitState(this, Hit, false);
        #endregion
    }

    private void Start()
    {
        currentState = idleState; //idle为默认姿态
        animator.Play(idleState.animationName);
    }

    private void Update()
    {
        //输入获取
        playerInputManager.GetAllInput();

        //持续调用当前姿态的Update方法
        currentState.Update();

        当前姿态 = currentState.ToString();

        if (Input.GetKeyDown(KeyCode.L))
        {
            TakeDamage(transform.forward,PlayerDamageType.Heavy);
        }
    }

    #region 可视化绘制
    private void OnDrawGizmos()
    {

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



        // 绘制攻击锁定敌人检测范围
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackDetectionRadius);

        //绘制攻击范围
        Gizmos.color = Color.red;
        Vector3 offsetPoint = transform.rotation * attackOffsetPoint;
        Gizmos.DrawWireSphere(transform.position + offsetPoint, attackRadius);

        //绘制范围攻击的范围
        Gizmos.color = Color.red;
        Vector3 areaOffsetPoint = transform.rotation * areaAttackOffsetPoint;
        Gizmos.DrawWireSphere(transform.position + areaOffsetPoint, areaAttackRaRadius);

        //绘制触碰敌人检测范围
        // 获取CharacterController的碰撞器中心
        Vector3 colliderCenter = characterController.bounds.center;

        // 计算球形检测的半径
        float detectionRadius = characterController.radius + attackTouchRadius;

        // 绘制检测范围（球体）
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(colliderCenter, detectionRadius);
    }

    #endregion


    #region 应用重力
    [Header("应用重力")]
    public Vector3 yVelocity; //垂直速度
    public float groundedYVelocity = -2f; //在地面上的垂直速度,使角色贴紧地面
    public float gravity; //重力加速度
    public bool useGravity = true; //是否需要应用重力效果
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
    #endregion


    #region 使用RootMotion的数据控制角色移动
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
    #endregion


    #region 地面检测
    [Header("地面检测")]
    public bool isGrounded;
    public LayerMask groundLayer; //检测层级
    public float groundCheckSphereRadius; //使用球型检测，检测的半径,与characterControll半径一致最为合适
    public bool GroundCheck()
    {
        Vector3 lowestPoint = new Vector3(
            transform.position.x,
            characterController.bounds.min.y + groundCheckSphereRadius - 0.05f, // 上移一点
            transform.position.z
        );

        return Physics.CheckSphere(lowestPoint, groundCheckSphereRadius, groundLayer);
    }
    #endregion


    #region 攻击时周围范围敌人锁定检测(获取攻击范围最近的敌人的坐标)
    [Header("攻击范围敌人锁定")]
    public float attackDetectionRadius; //检测半径
    public LayerMask attackDetectionLayer; //检测层级，一般为Enemy
    public Vector3 DetectionEnemyPositionInAttack()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, attackDetectionRadius, attackDetectionLayer);

        //检测到敌人
        if (enemies.Length > 0)
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
    #endregion


    #region 攻击姿态(如果攻击到敌人)限制rootMotion移动
    [Header("限制RootMotion")]
    public float attackTouchRadius; //接触范围半径
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
    #endregion


    #region 受击
    [Header("受击")]
    public float knockbackSpeedMul; //扩大动画曲线参数倍率
    public AnimationCurve damageLightBackMoveCurve; // 轻击后退速度曲线
    public AnimationCurve damageMediumBackMoveCurve; // 中击后退速度曲线
    public AnimationCurve damageHeavyBackMoveCurve; // 重击后退速度曲线
    //受到伤害
    public void TakeDamage(Vector3 _hitDirection, PlayerDamageType _damageType = PlayerDamageType.None)
    {
        //TODO:霸体判断

        //TODO:目前没有实现空中受击，只实现了地面受击的逻辑

        //根据受击类型选着不同的动画播放
        string animationName = "";
        switch (_damageType)
        {
            case PlayerDamageType.None:
                break;
            case PlayerDamageType.Light:
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
                currentState.ChangeState(groundHitState);
                //进行动画过渡
                animator.CrossFadeInFixedTime(animationName, 0.1f);
                groundHitState.damageType = _damageType;
                groundHitState.hitDir = _hitDirection;
                //注册受击语音事件
                (currentState as PlayerGroundHitState).action_playHitSound += (currentState as PlayerGroundHitState).PlayHitSound;
                break;
            case PlayerDamageType.Medium:
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
                currentState.ChangeState(groundHitState);
                //进行动画过渡
                animator.CrossFadeInFixedTime(animationName, 0.1f);
                groundHitState.damageType = _damageType;
                groundHitState.hitDir = _hitDirection;
                //注册受击语音事件
                (currentState as PlayerGroundHitState).action_playHitSound += (currentState as PlayerGroundHitState).PlayHitSound;
                break;
            case PlayerDamageType.Heavy:
                animationName = Hit_Down;
                // 切换为受击姿态
                currentState.ChangeState(groundHitState);
                //进行动画过渡
                animator.CrossFadeInFixedTime(animationName, 0.1f);
                groundHitState.damageType = _damageType;
                groundHitState.hitDir = _hitDirection;
                //注册受击语音事件
                (currentState as PlayerGroundHitState).action_playHitSound += (currentState as PlayerGroundHitState).PlayHitSound;
                break;
            case PlayerDamageType.FlyAway:

                break;
            case PlayerDamageType.AirLight:

                break;
            default:
                Debug.LogError("出错了...");
                break;
        }
    }

    #endregion











    //动画事件
    #region 攻击检测
    [Header("攻击范围")]
    public float attackRadius; //攻击半径
    public Vector3 attackOffsetPoint; //相对于自身位置偏移量
    public LayerMask attackTargetLayer; //攻击对象层级，一般为Enemy
    public float areaAttackRaRadius; //范围攻击半径
    public Vector3 areaAttackOffsetPoint; //范围攻击相对于自身位置偏移量
    public bool AttackDetecte(int _attackType)
    {
        // 将整数转换为 DamageType
        EnemyDamageType damageType = _attackType switch
        {
            0 => EnemyDamageType.None,
            1 => EnemyDamageType.Light,  // 轻击
            2 => EnemyDamageType.Medium,   // 中等攻击
            3 => EnemyDamageType.Heavy,    // 重击
            4 => EnemyDamageType.FlyAway, //击飞
            5 => EnemyDamageType.AirLight, //空中轻击
            _ => EnemyDamageType.None, //默认无攻击类型
        };

        // 计算攻击偏移位置
        Vector3 offsetPoint = transform.rotation * attackOffsetPoint;

        Collider[] enemys = Physics.OverlapSphere(transform.position + offsetPoint, attackRadius, attackTargetLayer);

        if (enemys.Length == 0)
        {
            return false;
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

        return true;
    }

    //范围攻击检测
    public bool AreaAttackDetecte(int _attackType)
    {
        // 将整数转换为 DamageType
        EnemyDamageType damageType = _attackType switch
        {
            0 => EnemyDamageType.None,
            1 => EnemyDamageType.AirLight, //空中受到范围轻击(较轻)
            2 => EnemyDamageType.Heavy, //受到范围重击
            _ => EnemyDamageType.None, //默认无攻击类型
        };

        // 计算攻击偏移位置
        Vector3 offsetPoint = transform.rotation * areaAttackOffsetPoint;

        Collider[] enemys = Physics.OverlapSphere(transform.position + offsetPoint, areaAttackRaRadius, attackTargetLayer);

        if (enemys.Length == 0)
        {
            return false;
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

        return true;
    }
    #endregion

    #region 播放音效
    //辅助方法：随机获取一个音效
    private string GetRandomAudioName(string[] _audioNames)
    {
        int index = Random.Range(0, _audioNames.Length);
        return _audioNames[index];
    }
    //辅助方法：是否播放
    private bool IsPlay(float _possibility)
    {
        float possibility = Random.Range(0f, 1f);
        return possibility < _possibility ? true : false;
    }
    //辅助方法：概率播放语音
    private void PlayPlayerSound(string[] _audioNames, float _possibility)
    {
        if (IsPlay(_possibility))
        {
            AudioManager.Instance.PlaySound(GetRandomAudioName(_audioNames),AudioType.Sound,transform.position);
        }
    }

    //播放闪避语音
    public void PlayPlayerDodgeSound()
    {
        string[] names = new string[] { AudioPathConfi.Sound_Dodge};
        PlayPlayerSound(names, 0.2f);
    }

    //播放跳跃语音
    public void PlayPlayerJumpSound()
    {
        string[] names = new string[] {AudioPathConfi.Sound_Jump1, AudioPathConfi.Sound_Jump2};
        PlayPlayerSound(names, 0.5f);
    }

    //播放跳跃落地音效
    public void PlayPlayerJumpEndSFX()
    {
        AudioManager.Instance.PlaySound(AudioPathConfi.SFX_JumpEnd, AudioType.SFX, transform.position);
    }

    //播放combo_1_1音效
    public void PlayCombo_1_1SFX()
    {
        AudioManager.Instance.PlaySound(AudioPathConfi.SFX_Combo_1_1, AudioType.SFX, transform.position);
    }
    //播放Combo_1_1语音
    public void PlayCombo_1_1Sound()
    {
        string[] names = new string[] { AudioPathConfi.Sound_Combo_1_1 };
        PlayPlayerSound(names, 0.5f);
    }

    //播放combo_1_2音效
    public void PlayCombo_1_2SFX()
    {
        AudioManager.Instance.PlaySound(AudioPathConfi.SFX_Combo_1_2, AudioType.SFX, transform.position);
    }
    //播放Combo_1_2语音
    public void PlayCombo_1_2Sound()
    {
        string[] names = new string[] { AudioPathConfi.Sound_Combo_1_2 };
        PlayPlayerSound(names, 0.5f);
    }

    //播放combo_1_3_1音效
    public void PlayCombo_1_3_1SFX()
    {
        AudioManager.Instance.PlaySound(AudioPathConfi.SFX_Combo_1_3_1, AudioType.SFX, transform.position);
    }
    //播放combo_1_3_2音效
    public void PlayCombo_1_3_2SFX()
    {
        AudioManager.Instance.PlaySound(AudioPathConfi.SFX_Combo_1_3_2, AudioType.SFX, transform.position);
    }
    //播放Combo_1_3语音
    public void PlayCombo_1_3Sound()
    {
        string[] names = new string[] { AudioPathConfi.Sound_Combo_1_3 };
        PlayPlayerSound(names, 0.5f);
    }

    //播放ChargeAttack_1音效
    public void PlayChargeAttack_1SFX()
    {
        AudioManager.Instance.PlaySound(AudioPathConfi.SFX_ChargeAttack_1, AudioType.SFX, transform.position);
    }
    //播放ChargeAttack_1语音
    public void PlayChargeAttack_1Sound()
    {
        string[] names = new string[] { AudioPathConfi.Sound_ChargeAttack_1 };
        PlayPlayerSound(names, 0.5f);
    }

    //播放combo_1_4音效
    public void PlayCombo_1_4SFX()
    {
        AudioManager.Instance.PlaySound(AudioPathConfi.SFX_Combo_1_4, AudioType.SFX, transform.position);
    }
    //播放Combo_1_4语音
    public void PlayCombo_1_4Sound()
    {
        string[] names = new string[] { AudioPathConfi.Sound_Combo_1_4 };
        PlayPlayerSound(names, 0.5f);
    }

    //播放combo_1_5_1音效
    public void PlayCombo_1_5_1SFX()
    {
        AudioManager.Instance.PlaySound(AudioPathConfi.SFX_Combo_1_5_1, AudioType.SFX, transform.position);
    }
    //播放combo_1_5_2音效
    public void PlayCombo_1_5_2SFX()
    {
        AudioManager.Instance.PlaySound(AudioPathConfi.SFX_Combo_1_5_2, AudioType.SFX, transform.position);
    }
    //播放Combo_1_5语音
    public void PlayCombo_1_5Sound()
    {
        string[] names = new string[] { AudioPathConfi.Sound_Combo_1_5 };
        PlayPlayerSound(names, 0.5f);
    }

    //播放SprintAttack_1音效
    public void PlaySprintAttack_1SFX()
    {
        AudioManager.Instance.PlaySound(AudioPathConfi.SFX_SprintAttack_1, AudioType.SFX, transform.position);
    }
    //播放SprintAttack_1语音
    public void PlaySprintAttack_1Sound()
    {
        string[] names = new string[] { AudioPathConfi.Sound_SprintAttack_1 };
        PlayPlayerSound(names, 0.5f);
    }

    //播放SprintAttack_2音效
    public void PlaySprintAttack_2SFX()
    {
        AudioManager.Instance.PlaySound(AudioPathConfi.SFX_SprintAttack_2, AudioType.SFX, transform.position);
    }
    //播放SprintAttack_2语音
    public void PlaySprintAttack_2Sound()
    {
        string[] names = new string[] { AudioPathConfi.Sound_SprintAttack_2 };
        PlayPlayerSound(names, 0.5f);
    }

    //播放AirCombo_1_1音效
    public void PlayAirCombo_1_1SFX()
    {
        AudioManager.Instance.PlaySound(AudioPathConfi.SFX_AirCombo_1_1, AudioType.SFX, transform.position);
    }
    //播放AirCombo_1_1语音
    public void PlayAirCombo_1_1Sound()
    {
        string[] names = new string[] { AudioPathConfi.Sound_AirCombo_1_1 };
        PlayPlayerSound(names, 0.5f);
    }

    //播放AirCombo_1_2音效
    public void PlayAirCombo_1_2SFX()
    {
        AudioManager.Instance.PlaySound(AudioPathConfi.SFX_AirCombo_1_2, AudioType.SFX, transform.position);
    }
    //播放AirCombo_1_2语音
    public void PlayAirCombo_1_2Sound()
    {
        string[] names = new string[] { AudioPathConfi.Sound_AirCombo_1_2 };
        PlayPlayerSound(names, 0.5f);
    }

    //播放AirCombo_1_3音效
    public void PlayAirCombo_1_3SFX()
    {
        AudioManager.Instance.PlaySound(AudioPathConfi.SFX_AirCombo_1_3, AudioType.SFX, transform.position);
    }
    //播放AirCombo_1_3语音
    public void PlayAirCombo_1_3Sound()
    {
        string[] names = new string[] { AudioPathConfi.Sound_AirCombo_1_3 };
        PlayPlayerSound(names, 0.5f);
    }

    //播放AirCombo_1_4音效
    public void PlayAirCombo_1_4SFX()
    {
        AudioManager.Instance.PlaySound(AudioPathConfi.SFX_AirCombo_1_4, AudioType.SFX, transform.position);
    }
    //播放AirCombo_1_4语音
    public void PlayAirCombo_1_4Sound()
    {
        string[] names = new string[] { AudioPathConfi.Sound_AirCombo_1_4 };
        PlayPlayerSound(names, 0.5f);
    }

    //播放PowerAttack语音
    public void PlayAirPowerAttackSound()
    {
        string[] names = new string[] { AudioPathConfi.Sound_PowerAttack_1, AudioPathConfi.Sound_PowerAttack_2 , AudioPathConfi.Sound_PowerAttack_3, AudioPathConfi.Sound_PowerAttack_4, AudioPathConfi.Sound_PowerAttack_5, AudioPathConfi.Sound_PowerAttack_6 };
        PlayPlayerSound(names, 1f);
    }

    //概率播放收到轻击的语音
    public void PlayLightHitSound()
    {
        string[] names = new string[] { AudioPathConfi.Sound_LightHit_1, AudioPathConfi.Sound_LightHit_2 };
        PlayPlayerSound(names, 0.3f);
    }
    //概率播放收到中击的语音
    public void PlayMediumHitSound()
    {
        string[] names = new string[] { AudioPathConfi.Sound_MediumHit_1, AudioPathConfi.Sound_MediumHit_1 };
        PlayPlayerSound(names, 0.6f);
    }
    //概率播放收到重击的语音
    public void PlayHeavyHitSound()
    {
        string[] names = new string[] { AudioPathConfi.Sound_HeavyHit_1,AudioPathConfi.Sound_HeavyHit_2 };
        PlayPlayerSound(names, 0.8f);
    }
    #endregion
}
