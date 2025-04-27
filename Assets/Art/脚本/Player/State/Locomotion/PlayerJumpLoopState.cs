using UnityEngine;

public class PlayerJumpLoopState : PlayerJumpState
{
    public PlayerJumpLoopState(PlayerManager _playerManager, string _animationName, bool _useRootMotionPart) : base(_playerManager, _animationName, _useRootMotionPart)
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



        //���ʱ������ƶ����룬��������ض����㼶��Ȩ��

        if (playerManager.isGrounded && playerManager.yVelocity.y < 0f)
        {
            //���ʱ���ƶ����������
            if(playerManager.playerInputManager.moveRawAmount == 0)
            {
                //jumpLoop -> jumpStop
                ChangeState(playerManager.jumpStopState);
                playerManager.animator.CrossFade(playerManager.jumpStopState.animationName, 0.25f);
            }
            else //���ʱ���ƶ�����
            {
                //jumpLoop -> sprint
                if(jumpMoveSpeed == playerManager.jumpSprintMoveSpeed)
                {
                    ChangeState(playerManager.sprintState);
                    playerManager.animator.CrossFade(playerManager.sprintState.animationName, 0.25f);
                    playerManager.animator.SetLayerWeight(1, 0.3f);
                    playerManager.animator.CrossFade(PlayerAnimationName.JumpStop, 0.1f, 1);

                }
                //jumpLoop -> run
                else if (playerManager.playerInputManager.runInput)
                {
                    ChangeState(playerManager.runState);
                    playerManager.animator.CrossFade(playerManager.runState.animationName, 0.25f);
                    playerManager.animator.SetLayerWeight(1, 0.2f);
                    playerManager.animator.CrossFade(PlayerAnimationName.JumpStop, 0.1f, 1);

                }
                //jumpLoop -> walk
                else if (!playerManager.playerInputManager.runInput)
                {
                    ChangeState(playerManager.walkState);
                    playerManager.animator.CrossFade(playerManager.walkState.animationName, 0.25f);
                    playerManager.animator.SetLayerWeight(1, 0.1f);
                    playerManager.animator.CrossFade(PlayerAnimationName.JumpStop, 0.1f, 1);

                }
            }
        }
        //��������̬�а�����������������е�һ���չ�: jumpLoop -> airCombo_1_1
        else if (playerManager.playerInputManager.Mouse0DownInput()) 
        {
            ChangeState(playerManager.airCombo_1_1State);
            playerManager.animator.CrossFade(playerManager.airCombo_1_1State.animationName,0.1f);
            playerManager.yVelocity.y = playerManager.groundedYVelocity;
        }
        //��������̬�н������������룬�������Ӧ������̬: jump -> airDodge
        else if (playerManager.playerInputManager.DodgeInput())
        {
            ChangeState(playerManager.airDodgeState);
            playerManager.yVelocity.y = playerManager.groundedYVelocity;
        }
    }
}
