using UnityEngine;

public class EnemyGetUpState : EnemyState
{
    public EnemyGetUpState(EnemyManager _enemyManager, string _animationName, bool _useRootMotionPart) : base(_enemyManager, _animationName, _useRootMotionPart)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }
    //test

    public override void Exit()
    {
        base.Exit();
    }

    //...2222
    public override void Update()
    {
        base.Update();

        AnimatorStateInfo stateInfo = enemyManager.animator.GetCurrentAnimatorStateInfo(0);
        //¶É³ö: this -> idle
        if(stateInfo.IsName(animationName) && enemyManager.animator.IsInTransition(0))
        {
            ChangeState(enemyManager.idleState);
        }
    }
}
