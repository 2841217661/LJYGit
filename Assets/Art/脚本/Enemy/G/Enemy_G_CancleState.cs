using UnityEngine;

public class Enemy_G_CancleState : EnemyGuardState
{
    public Enemy_G_CancleState(EnemyManager _enemyManager, string _animationName, bool _useRootMotionPart) : base(_enemyManager, _animationName, _useRootMotionPart)
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

        AnimatorStateInfo stateInfo = enemyManager.animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName(animationName) && enemyManager.animator.IsInTransition(0))
        {
            ChangeState(enemyManager.G_idleState);
            enemyManager.animator.CrossFadeInFixedTime(enemyManager.G_idleState.animationName, 0.2f);
        }
    }
}
