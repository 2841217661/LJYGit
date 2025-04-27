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

        ////���ڵ�����
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
        // ������ player.position �����ҵ�һ����Ч�� NavMesh ��
        if (NavMesh.SamplePosition(enemyManager.player.position, out hit, 5.0f, NavMesh.AllAreas))
        {
            enemyManager.agent.SetDestination(hit.position);
            return true;
        }

        Debug.LogWarning("�޷�����Ŀ�꣺��Ҳ��ڵ���������");
        return false;
    }


}
