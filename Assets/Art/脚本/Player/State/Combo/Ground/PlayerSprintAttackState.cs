using System;
using UnityEngine;

public class PlayerSprintAttackState : PlayerGroundComboState
{
    //���ݶ���ʱ���Ⱦ��������˳�ʱ��
    private float attack_1Exit = 0.3f; 
    private float attack_2Exit = 0.5f;

    private Action action_sprintAttackEnemy; //�ڳ�̹�������ֻ��Ե������һ���˺�

    public PlayerSprintAttackState(PlayerManager _playerManager, string _animationName, bool _useRootMotionPart) : base(_playerManager, _animationName, _useRootMotionPart)
    {
    }

    public override void Enter()
    {
        base.Enter();

        action_sprintAttackEnemy += AttackEnemy;
    }

    public override void Exit()
    {
        base.Exit();

        action_sprintAttackEnemy -= AttackEnemy;
    }

    public override void Update()
    {
        base.Update();

        action_sprintAttackEnemy?.Invoke();


        //��ȡ����״̬��״̬
        AnimatorStateInfo stateInfo = playerManager.animator.GetCurrentAnimatorStateInfo(0);

        //״̬�˳����
        if (stateInfo.IsTag(animationName))
        {
            //sprintAttack_1 -> idle
            if (stateInfo.IsName(PlayerAnimationName.SprintAttack_1))
            {
                if (stateInfo.normalizedTime > attack_1Exit)
                {
                    //���ƶ����룬ֱ���л�Ϊ������̬
                    if (playerManager.playerInputManager.moveRawAmount != 0)
                    {
                        ChangeState(playerManager.idleState);
                        playerManager.animator.CrossFadeInFixedTime(playerManager.idleState.animationName, 0.2f);
                    }
                    else
                    {
                        //�����Ѿ�������ϣ����ڽ��й���
                        if (playerManager.animator.IsInTransition(0))
                        {
                            ChangeState(playerManager.idleState);
                            playerManager.animator.CrossFadeInFixedTime(playerManager.idleState.animationName, 0.2f);
                        }
                    }
                }
            }
            //sprintAttack_2 -> idle
            else if (stateInfo.IsName(PlayerAnimationName.SprintAttack_2))
            {
                if (stateInfo.normalizedTime > attack_2Exit)
                {
                    //���ƶ����룬ֱ���л�Ϊ������̬
                    if (playerManager.playerInputManager.moveRawAmount != 0)
                    {
                        ChangeState(playerManager.idleState);
                        playerManager.animator.CrossFadeInFixedTime(playerManager.idleState.animationName, 0.2f);
                    }
                    else
                    {
                        //�����Ѿ�������ϣ����ڽ��й���
                        if (playerManager.animator.IsInTransition(0))
                        {
                            ChangeState(playerManager.idleState);
                        }
                    }
                }
            }
        }
    }

    //��̹��̹�������
    private void AttackEnemy()
    {
        if (playerManager.AttackDetecte(3))
        {
            //�����Һ͵���ͬʱ����ͻ������������ڹ��������˵�˲�䣬���˽���hit��̬����ʱ������ײ�����һ�����غϣ�
            //���������ײ���ǹ�̬�ģ��ͻᵼ�µ������л���hitʱ������

            //������ײ��������hit��̬����ʱ��ָ���ײ
            Physics.IgnoreLayerCollision(7, 8, true);

            action_sprintAttackEnemy -= AttackEnemy;
        }
    }
}
