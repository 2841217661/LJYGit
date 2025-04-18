using UnityEngine;
using UnityEngine.UIElements;

public class EnemyGuardState : EnemyState
{
    public EnemyGuardState(EnemyManager _enemyManager, string _animationName, bool _useRootMotionPart) : base(_enemyManager, _animationName, _useRootMotionPart)
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

        //����ھ��䷶Χ��⵽��Player������׷����̬(�ε�)
        enemyManager.guardTargetManager = enemyManager.GuardDetectePlayer();

        if(enemyManager.guardTargetManager != null)
        {
            ChangeState(enemyManager.C_prepareState);
        }
    }
}
