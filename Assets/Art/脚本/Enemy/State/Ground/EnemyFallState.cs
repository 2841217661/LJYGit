using UnityEngine;
public class EnemyFallState : EnemyAirState
{
    public EnemyFallState(EnemyManager _enemyManager, string _animationName, bool _useRootMotionPart) : base(_enemyManager, _animationName, _useRootMotionPart)
    {
    }

    public override void Enter()
    {
        base.Enter();

    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (enemyManager.IsGrounded() && enemyManager.rb.linearVelocity.y <= 0f)
        {
            ChangeState(enemyManager.idleState);
            enemyManager.animator.CrossFadeInFixedTime(enemyManager.idleState.animationName, 0.2f);
            return;
        }
    }
}
