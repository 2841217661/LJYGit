using UnityEngine;
using UnityEngine.AI;

public class EnemyGroundState : EnemyState
{
    public EnemyGroundState(EnemyManager _enemyManager, string _animationName, bool _useRootMotionPart) : base(_enemyManager, _animationName, _useRootMotionPart)
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

        ////不在地面上
        //if (!enemyManager.IsGrounded() && enemyManager.currentState != enemyManager.airHitState && enemyManager.currentState != enemyManager.hitState)
        //{
        //    ChangeState(enemyManager.fallState);
        //    enemyManager.animator.CrossFadeInFixedTime(enemyManager.fallState.animationName, 0.2f);
        //    return;
        //}
    }

    public bool TrySetDestinationToPlayer()
    {
        NavMeshHit hit;
        // 尝试在 player.position 附近找到一个有效的 NavMesh 点
        if (NavMesh.SamplePosition(enemyManager.player.position, out hit, 5.0f, NavMesh.AllAreas))
        {
            enemyManager.agent.SetDestination(hit.position);
            return true;
        }

        Debug.LogWarning("无法设置目标：玩家不在导航区域内");
        return false;
    }


}
