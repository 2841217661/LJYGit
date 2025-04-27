using UnityEngine;

public class PlayerGroundState : PlayerState
{
    public PlayerGroundState(PlayerManager _playerManager, string _animationName, bool _useRootMotionPart) : base(_playerManager, _animationName, _useRootMotionPart)
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

        playerManager.HandleGravity();

        //��Ծ������
        //���ݵ�ǰ����̬idle,walk,run,sprint����jump��̬���ƶ��ٶ�
        if (playerManager.playerInputManager.JumpInput())
        {
            //idle,walkStop,runStop,sprintStop
            if (playerManager.currentState is PlayerIdleState || playerManager.currentState is PlayerWalkState)
            {
                playerManager.jumpStartState.jumpMoveSpeed = playerManager.jumpWalkMoveSpeed;
                playerManager.jumpLoopState.jumpMoveSpeed = playerManager.jumpWalkMoveSpeed;
            }
            else if (playerManager.currentState == playerManager.runState)
            {
                playerManager.jumpStartState.jumpMoveSpeed = playerManager.jumpRunMoveSpeed;
                playerManager.jumpLoopState.jumpMoveSpeed = playerManager.jumpRunMoveSpeed;
            }
            else if (playerManager.currentState == playerManager.sprintState)
            {
                playerManager.jumpStartState.jumpMoveSpeed = playerManager.jumpSprintMoveSpeed;
                playerManager.jumpLoopState.jumpMoveSpeed = playerManager.jumpSprintMoveSpeed;
            }

            ChangeState(playerManager.jumpStartState);
            return;
        }

        //���浽���У����䣩���
        else if (!playerManager.isGrounded)
        {
            //ground -> jumpLoop
            if(playerManager.inAirTimer > 0.1f)
            {
                ChangeState(playerManager.jumpLoopState);
                playerManager.animator.CrossFadeInFixedTime(playerManager.jumpLoopState.animationName, 0.25f);
                return;
            }
        }

        //�չ��������
        else if (playerManager.playerInputManager.Mouse0DownInput())
        {
            //��һ�����⣬������ڵ�һ�¹����󲢰�ס�ƶ������ڹ�������ʱӦ���л���run(������ҵ�ǰ��ʹ�õ�run�����ƶ�)
            //������Ҿ����ڴ�combo_1 -> run������ɹ����з�����������ᵼ��combo_1 -> combo_1�Ĺ��ɣ�
            //��combo_1�ж��߼�������ͻ����Ҫ��һ������
            AnimatorStateInfo stateInfo = playerManager.animator.GetCurrentAnimatorStateInfo(0);
            if (!stateInfo.IsName(playerManager.combo_1_1State.animationName))
            {
                //idle/walk/run -> combo_1_1
                ChangeState(playerManager.combo_1_1State);
                playerManager.animator.CrossFadeInFixedTime(playerManager.combo_1_1State.animationName, 0.2f);
                return;
            }
        }
        //powerAttack������: groundState -> powerAttackStart
        else if (playerManager.playerInputManager.EDownInput())
        {
            ChangeState(playerManager.powerAttackStartState);
            playerManager.animator.CrossFadeInFixedTime(playerManager.powerAttackStartState.animationName, 0.1f);
            return;
        }
    }
}
