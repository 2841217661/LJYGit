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



        //落地时如果有移动输入，则设置落地动画层级的权重

        if (playerManager.isGrounded && playerManager.yVelocity.y < 0f)
        {
            //落地时无移动方向的输入
            if(playerManager.playerInputManager.moveRawAmount == 0)
            {
                //jumpLoop -> jumpStop
                ChangeState(playerManager.jumpStopState);
                playerManager.animator.CrossFade(playerManager.jumpStopState.animationName, 0.25f);
            }
            else //落地时有移动输入
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
        //在下落姿态中按下了鼠标左键进入空中第一阶普攻: jumpLoop -> airCombo_1_1
        else if (playerManager.playerInputManager.Mouse0DownInput()) 
        {
            ChangeState(playerManager.airCombo_1_1State);
            playerManager.animator.CrossFade(playerManager.airCombo_1_1State.animationName,0.1f);
            playerManager.yVelocity.y = playerManager.groundedYVelocity;
        }
        //在下落姿态中进行了闪避输入，则进入相应闪避姿态: jump -> airDodge
        else if (playerManager.playerInputManager.DodgeInput())
        {
            ChangeState(playerManager.airDodgeState);
            playerManager.yVelocity.y = playerManager.groundedYVelocity;
        }
    }
}
