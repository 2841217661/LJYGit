using System.Collections;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private string 当前姿态;

    [HideInInspector] public Animator animator;
    [HideInInspector] public Rigidbody rg;
    [HideInInspector] public CapsuleCollider capsuleCollider;

    public GameObject weapon;

    [Header("警戒Player")]
    public float guardRadius;
    public Vector3 guardCentPointOffset; //警戒范围中心为自身位置+offset
    public LayerMask guardLayer;
    public Transform guardPointParent;
    public float guardWalkSpeed;
    public float guardRotateSpeed;
    public PlayerManager guardTargetManager;

    [Header("受击")]
    public float knockbackSpeedMul; //扩大动画曲线参数倍率
    public float maxKnockbackThreshold; //最大击退阈值
    public float currentKnockbackThreshold; //当前击退阈值
    public float knockbackRecoveMul; //击退阈值恢复倍率
    public AnimationCurve damageLightBackMoveCurve; // 轻击后退速度曲线
    public AnimationCurve damageMediumBackMoveCurve; // 中击后退速度曲线
    public AnimationCurve damageHeavyBackMoveCurve; // 重击后退速度曲线
    public float minDamageLightReduceKnockbackThreshold; //轻击减小阈值最小值
    public float maxDamageLightReduceKnockbackThreshold; //轻击减小阈值最大值
    public float minDamageMediumReduceKnockbackThreshold;//中击减小阈值小值
    public float maxDamageMediumReduceKnockbackThreshold;//中击减小阈值大值
    public float minDamageHeavyReduceKnockbackThreshold;//重击减小阈值最小值
    public float maxDamageHeavyReduceKnockbackThreshold;//重击减小阈值最大值
    public enum DamageType // 受击类型
    {
        None, // 某些敌人无法被击退
        Light, // 轻击攻击
        Medium, // 不重不轻的攻击
        Heavy, // 重击攻击
    }

    public EnemyState currentState;
    public Enemy_G_IdleState G_idleState { get; private set; }
    public Enemy_G_HitState G_hitState { get; private set; }
    public Enemy_G_GetUpState G_getUpState { get; private set; }
    public Enemy_G_WalkState G_walkState{ get; private set; }
    public Enemy_G_CancleState G_cancleState { get; private set; }
    public Enemy_C_PrepareState C_prepareState { get; private set; }
    public Enemy_C_IdleState C_idleState { get; private set; }
    private void Awake()
    {
        animator = GetComponent<Animator>();
        rg = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        G_idleState = new Enemy_G_IdleState(this, "G_Idle", false);
        G_hitState = new Enemy_G_HitState(this, "Hit", false);
        G_getUpState = new Enemy_G_GetUpState(this, "Get_Up", false);
        G_walkState = new Enemy_G_WalkState(this, "Walk", false);
        C_prepareState = new Enemy_C_PrepareState(this, "Prepare", false);
        C_idleState = new Enemy_C_IdleState(this, "C_Idle", false);
        G_cancleState = new Enemy_G_CancleState(this, "Cancle", false);
    }

    private void Start()
    {
        currentState = G_idleState;

        currentKnockbackThreshold = maxKnockbackThreshold;
    }

    private void Update()
    {
        当前姿态 = currentState.ToString();
        currentState.Update();

        RecoveKnockbackThreshold();
    }

    private void OnAnimatorMove()
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
        if (rg != null && !rg.isKinematic)
        {
            // 使用Rigidbody移动和旋转（必须在FixedUpdate中才生效）
            rg.MovePosition(rg.position + rootMotionDelta);
            rg.MoveRotation(rg.rotation * rootRotationDelta);
        }
        else
        {
            // 如果是Kinematic Rigidbody 或者没加刚体，就直接修改Transform
            transform.position += rootMotionDelta;
            transform.rotation *= rootRotationDelta;
        }
    }


    /// <summary>
    /// 敌人受击逻辑
    /// </summary>
    /// <param name="_hitDirection">受击方向，由伤害来源指向自己</param>
    /// <param name="_damageType">受击类型</param>
    public void TakeDamage(Vector3 _hitDirection, DamageType _damageType = DamageType.None)
    {
        //根据受击类型选着不同的动画播放
        string animationName = "";
        switch (_damageType)
        {
            case DamageType.None:
                break;
            case DamageType.Light:
                if (RandomReducceKnockbakcThreshold(minDamageLightReduceKnockbackThreshold,maxDamageLightReduceKnockbackThreshold))
                {
                    // 根据受伤方向播放不同方向的受击动画
                    float hitAngle = Vector3.SignedAngle(new Vector3(transform.forward.x, 0f, transform.forward.z), _hitDirection, Vector3.up);
                    animationName = hitAngle switch
                    {
                        >= -45f and <= 45f => "HitB",
                        >= -135f and < -45f => "HitL",
                        > 45f and <= 135f => "HitR",
                        _ => "HitF"
                    };
                }
                else
                {
                    return;
                }
                break;
            case DamageType.Medium:
                if (RandomReducceKnockbakcThreshold(minDamageMediumReduceKnockbackThreshold,maxDamageMediumReduceKnockbackThreshold))
                {
                    // 根据受伤方向播放不同方向的受击动画
                    float hitAngle1 = Vector3.SignedAngle(new Vector3(transform.forward.x, 0f, transform.forward.z), _hitDirection, Vector3.up);
                    animationName = hitAngle1 switch
                    {
                        >= -45f and <= 45f => "Hit_Large_B",
                        >= -135f and < -45f => "Hit_Large_L",
                        > 45f and <= 135f => "Hit_Large_R",
                        _ => "Hit_Large_F"
                    };
                }
                else
                {
                    return;
                }
                break;
            case DamageType.Heavy:
                if (RandomReducceKnockbakcThreshold(minDamageHeavyReduceKnockbackThreshold,maxDamageHeavyReduceKnockbackThreshold))
                {
                    animationName = "Hit_Large_Heavy";
                }
                else
                {
                    return;
                }
                break;
            default:
                Debug.LogError("出错了...");
                break;
        }

        // 切换为受击姿态
        currentState.ChangeState(G_hitState);
        //进行动画过渡
        animator.CrossFadeInFixedTime(animationName, 0.1f);

        (currentState as Enemy_G_HitState).damageType = _damageType;
        (currentState as Enemy_G_HitState).hitDir = _hitDirection;
    }

    //持续恢复击退阈值
    private void RecoveKnockbackThreshold()
    {
        currentKnockbackThreshold += Time.deltaTime * knockbackRecoveMul;

        if(currentKnockbackThreshold > maxKnockbackThreshold)
        {
            currentKnockbackThreshold = maxKnockbackThreshold;
        }
    }

    //随机减少击退阈值
    private bool RandomReducceKnockbakcThreshold(float _minValue, float _maxValue)
    {
        currentKnockbackThreshold -= Random.Range(_minValue, _maxValue);

        if(currentKnockbackThreshold <= 0)
        {
            currentKnockbackThreshold = 0;
            return true;
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        //绘制警戒范围
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + guardCentPointOffset, guardRadius);
    }

    //警戒Player
    public PlayerManager GuardDetectePlayer()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position + guardCentPointOffset, guardRadius, guardLayer); 

        if(colliders.Length > 0)
        {
            return colliders[0].GetComponent<PlayerManager>();
        }

        return null;
    }












    #region 动画事件

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
}