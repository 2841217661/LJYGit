using System;
using UnityEngine;
using static EnemyAnimationName;
public class EnemyHitState : EnemyGroundState
{
    public EnumManager.EnemyDamageType damageType;
    public Vector3 hitDir;

    public Action action_generateDamageEffect; //受击特效实例委托

    public EnemyHitState(EnemyManager _enemyManager, string _animationName, bool _useRootMotionPart) : base(_enemyManager, _animationName, _useRootMotionPart)
    {
    }

    public override void Enter()
    {
        base.Enter();

        enemyManager.rb.linearVelocity = Vector3.zero; //防止出现bug

    }

    public override void Exit()
    {
        base.Exit();

        //退出时清空受击枚举
        damageType = EnumManager.EnemyDamageType.None;
        hitDir = Vector3.zero;

        //恢复碰撞
        Physics.IgnoreLayerCollision(7, 8, false);
    }

    public override void Update()
    {
        base.Update();

        action_generateDamageEffect?.Invoke();

        AnimatorStateInfo stateInfo = enemyManager.animator.GetCurrentAnimatorStateInfo(0);
        AnimatorStateInfo nextStateInfo = enemyManager.animator.GetNextAnimatorStateInfo(0);

        //this -> idle(处于hit姿态过渡idle期间)
        if (stateInfo.IsTag(Hit) && nextStateInfo.IsName(enemyManager.idleState.animationName))
        {
            ChangeState(enemyManager.idleState);
        }
        else
        {
            SelectMoveAction(stateInfo.normalizedTime);
        }
    }

    private void SelectMoveAction(float _stateInfoNormalizedTime)
    {
        AnimatorStateInfo stateInfo = enemyManager.animator.GetCurrentAnimatorStateInfo(0);
        AnimatorStateInfo nextStateInfo = enemyManager.animator.GetNextAnimatorStateInfo(0);
        if (stateInfo.IsName(Get_Up) || nextStateInfo.IsName(Get_Up)) return;

        switch (damageType)
        {
            case EnumManager.EnemyDamageType.None:
                // 无位移
                Debug.Log("无法进行受击后退位移...");
                break;

            case EnumManager.EnemyDamageType.Light:
                HanleMovement(enemyManager.damageLightBackMoveCurve, _stateInfoNormalizedTime);
                break;

            case EnumManager.EnemyDamageType.Medium:
                HanleMovement(enemyManager.damageMediumBackMoveCurve, _stateInfoNormalizedTime);
                break;

            case EnumManager.EnemyDamageType.Heavy:
                HanleMovement(enemyManager.damageHeavyBackMoveCurve, _stateInfoNormalizedTime);
                break;
            default:
                Debug.LogError("错误,无该类型...");
                break;
        }
    }

    private void HanleMovement(AnimationCurve _curve, float _stateInfoNormalizedTime)
    {
        float moveAmount = _curve.Evaluate(_stateInfoNormalizedTime);

        Vector3 moveVector = hitDir * moveAmount * enemyManager.knockbackSpeedMul * Time.deltaTime;

        // 平滑移动：如果使用 Rigidbody，则推荐用 MovePosition
        enemyManager.rb.MovePosition(enemyManager.rb.position + moveVector);
    }

    public void GenerateDamageEffect()
    {

        enemyManager.GenerateDamageEffect();

        //注销委托
        action_generateDamageEffect -= GenerateDamageEffect;
    }
}
