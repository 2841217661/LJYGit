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
        enemyManager.agent.updateRotation = true; // ȷ��������ת����
        enemyManager.agent.updateUpAxis = true; // ȷ����������
    }

    public override void Exit()
    {
        base.Exit();

        enemyManager.agent.enabled = false;
    }

    public override void Update()
    {
        base.Update();

        // ��������Ŀ��Ϊ���λ��
        TrySetDestinationToPlayer();

        //�������̽��Χ�ڣ�
        if (enemyManager.DetecteIsInExploreRange())
        {
            float possibility = Random.Range(0, 1f);
            //this -> dodge/idle/roll_F/dashAttack
            if (possibility < 0.2f)
            {
                //this -> dodge_F:20%��ǰ����
                enemyManager.actionState.actionAnimationName = Dodge_F;
                ChangeState(enemyManager.actionState);
            }
            else if (possibility < 0.3f)
            {
                //this -> idle: 30%�ص�idle
                enemyManager.animator.CrossFadeInFixedTime(enemyManager.idleState.animationName, 0.2f);
                ChangeState(enemyManager.idleState);
            }
            else if (possibility < 0.6f)
            {
                //this -> roll_F: 30%��ǰ����
                enemyManager.actionState.actionAnimationName = Roll_F;
                ChangeState(enemyManager.actionState);
            }
            else
            {
                //this -> dashAttack: 40%��ǰͻ��
                enemyManager.actionState.actionAnimationName = Dash_Attack;
                ChangeState(enemyManager.actionState);
            }
        }

        //TODO:������ɣ�����sprint��̬
    }

}
