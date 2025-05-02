using UnityEngine;
using static EnemyAnimationName;
public class EnemyRunState : EnemyGroundState
{
    public EnemyRunState(EnemyManager _enemyManager, string _animationName, bool _useRootMotionPart) : base(_enemyManager, _animationName, _useRootMotionPart)
    {
    }

    public override void Enter()
    {
        base.Enter();

        enemyManager.agent.enabled = true;

        enemyManager.agent.speed = enemyManager.runSpeed;
        enemyManager.agent.angularSpeed = enemyManager.runRotateSpeed;
        enemyManager.agent.updateRotation = true; // 确保启用旋转更新
        enemyManager.agent.updateUpAxis = true; // 确保更新上轴
    }

    public override void Exit()
    {
        base.Exit();

        enemyManager.agent.enabled = false;
    }

    public override void Update()
    {
        base.Update();

        // 持续设置目标为玩家位置
        TrySetDestinationToPlayer();

        //到达可试探范围内：
        if (enemyManager.DetecteIsInExploreRange())
        {
            float possibility = Random.Range(0, 1f);
            //this -> dodge/idle/roll_F/dashAttack
            if (possibility < 0.2f)
            {
                //this -> dodge_F:20%向前闪避
                enemyManager.actionState.actionAnimationName = Dodge_F;
                ChangeState(enemyManager.actionState);
            }
            else if (possibility < 0.3f)
            {
                //this -> idle: 30%回到idle
                enemyManager.animator.CrossFadeInFixedTime(enemyManager.idleState.animationName, 0.2f);
                ChangeState(enemyManager.idleState);
            }
            else if (possibility < 0.6f)
            {
                //this -> roll_F: 30%向前翻滚
                enemyManager.actionState.actionAnimationName = Roll_F;
                ChangeState(enemyManager.actionState);
            }
            else
            {
                //this -> dashAttack: 40%向前突击
                enemyManager.actionState.actionAnimationName = Dash_Attack;
                ChangeState(enemyManager.actionState);
            }
        }

        //TODO:条件达成，进入sprint姿态
    }

}
