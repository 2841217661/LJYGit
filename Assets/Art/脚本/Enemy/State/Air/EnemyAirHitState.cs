using System;
using UnityEngine;
using static EnemyAnimationName;
public class EnemyAirHitState : EnemyAirState
{
    public Action action_addFlyForce; //地面受击飞起的委托
    public Action action_addAirForce; //空中受到攻击的委托

    private float flyForce = 450f;
    private float airForce = 3f;

    public EnumManager.EnemyDamageType damageType;
    public EnemyAirHitState(EnemyManager _enemyManager, string _animationName, bool _useRootMotionPart) : base(_enemyManager, _animationName, _useRootMotionPart)
    {
    }

    public override void Enter()
    {
        base.Enter();

    }

    public override void Exit()
    {
        base.Exit();

        damageType = EnumManager.EnemyDamageType.None;
    }

    public override void Update()
    {
        base.Update();

        //落到地面上
        if (enemyManager.IsGrounded() && enemyManager.rb.linearVelocity.y <= 0f && action_addFlyForce == null && action_addAirForce == null)
        {
            enemyManager.animator.CrossFade(Hit_Air_End, 0.1f);
            ChangeState(enemyManager.hitState);
        }
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();

        switch (damageType)
        {
            case EnumManager.EnemyDamageType.FlyAway:
                action_addFlyForce?.Invoke();
                break;
            case EnumManager.EnemyDamageType.AirLight:
                action_addAirForce?.Invoke();
                break;
            default:
                Debug.LogError("错误类型");
                break;
        }
    }

    //委托方法：受到击飞类型的攻击时，施加一次向上的力
    public void AddFlyForceOnce()
    {
        enemyManager.rb.linearVelocity = Vector3.zero;
        enemyManager.rb.AddForce(Vector3.up * flyForce, ForceMode.Impulse);
        // 移除这个方法，确保只执行一次
        action_addFlyForce -= AddFlyForceOnce;
    }

    //委托方法：在空中受到攻击时，施加一次向上的力
    public void AddAirForceOnce()
    {
        enemyManager.rb.linearVelocity = new Vector3(0, airForce, 0f);

        // 移除这个方法，确保只执行一次
        action_addAirForce -= AddAirForceOnce;

        if (enemyManager.player == null)
        {
            Debug.Log("player为空");
            return;
        }

        // 计算目标位置
        Vector3 targetPos = enemyManager.player.position + enemyManager.player.forward * 1f;

        // 做一次碰撞检测
        if (!Physics.Raycast(enemyManager.player.position, enemyManager.player.forward, out RaycastHit hit, 1f, LayerMask.GetMask("Default")))
        {
            // 如果前方没有障碍物，可以移动
            enemyManager.rb.MovePosition(targetPos);
        }
        else
        {
            // 有墙，就停在原地或者做其他处理
            Debug.Log("前方有障碍，不能吸附");
        }
    }
}
