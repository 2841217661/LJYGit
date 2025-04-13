using UnityEngine;

public class EnemyHitState : EnemyState
{
    public EnemyManager.DamageType damageType;
    public Vector3 hitDir;
    public EnemyHitState(EnemyManager _enemyManager, string _animationName, bool _useRootMotionPart) : base(_enemyManager, _animationName, _useRootMotionPart)
    {
    }

    public override void Enter()
    {
        base.Enter();

        enemyManager.currentKnockbackThreshold = enemyManager.maxKnockbackThreshold;
    }

    public override void Exit()
    {
        base.Exit();

        //退出时清空受击枚举
        damageType =EnemyManager.DamageType.None;
        hitDir = Vector3.zero;
    }

    public override void Update()
    {
        base.Update();

        AnimatorStateInfo stateInfo = enemyManager.animator.GetCurrentAnimatorStateInfo(0);
        AnimatorStateInfo nextStateInfo = enemyManager.animator.GetNextAnimatorStateInfo(0);

        //hit -> idle
        if (stateInfo.IsTag("Hit") && nextStateInfo.IsName(enemyManager.idleState.animationName))
        {
            ChangeState(enemyManager.idleState);
        }
        //hit -> getUp
        else if(stateInfo.IsTag("Hit") && nextStateInfo.IsName(enemyManager.getUpState.animationName))
        {
            ChangeState(enemyManager.getUpState);
        }
        else
        {
            SelectMoveAction(stateInfo.normalizedTime);
        }
    }

    private void SelectMoveAction(float _stateInfoNormalizedTime)
    {
        switch (damageType)
        {
            case EnemyManager.DamageType.None:
                // 无位移
                Debug.Log("无法进行受击后退位移...");
                break;

            case EnemyManager.DamageType.Light:
                HanleMovement(enemyManager.damageLightBackMoveCurve, _stateInfoNormalizedTime);
                break;

            case EnemyManager.DamageType.Medium:
                HanleMovement(enemyManager.damageMediumBackMoveCurve, _stateInfoNormalizedTime);
                break;

            case EnemyManager.DamageType.Heavy:
                HanleMovement(enemyManager.damageHeavyBackMoveCurve, _stateInfoNormalizedTime);
                break;

            default:
                Debug.LogError("错误,无该类型...");
                break;
        }
    }

    private void HanleMovement(AnimationCurve _curve, float _stateInfoNormalizedTime)
    {
        // 计算当前帧的移动量
        float moveAmount = _curve.Evaluate(_stateInfoNormalizedTime);

        // 使用 Rigidbody 来平滑移动物体
        Vector3 targetPosition = enemyManager.transform.position + hitDir * moveAmount * enemyManager.knockbackSpeedMul * Time.deltaTime;

        // 如果有 Rigidbody，可以使用 MovePosition 来平滑移动
        enemyManager.rg.MovePosition(targetPosition);
    }

}
