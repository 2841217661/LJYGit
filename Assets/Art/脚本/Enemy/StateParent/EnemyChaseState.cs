using UnityEngine;

public class EnemyChaseState : EnemyState
{
    public EnemyChaseState(EnemyManager _enemyManager, string _animationName, bool _useRootMotionPart) : base(_enemyManager, _animationName, _useRootMotionPart)
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

        //Àë¿ª¾¯½ä·¶Î§,´¥·¢ÊÕµ¶×ËÌ¬
        if (enemyManager.GuardDetectePlayer() == null)
        {
            enemyManager.guardTargetManager = null;
            ChangeState(enemyManager.G_cancleState);

            enemyManager.animator.CrossFadeInFixedTime(enemyManager.G_cancleState.animationName, 0.2f);
        }
    }
}
