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

        //跳跃输入检测
        //根据当前的姿态idle,walk,run,sprint决定jump姿态的移动速度
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
        }

        //地面到空中（下落）检测
        else if (!playerManager.isGrounded)
        {
            //ground -> jumpLoop
            if(playerManager.inAirTimer > 0.1f)
            {
                ChangeState(playerManager.jumpLoopState);
                playerManager.animator.CrossFadeInFixedTime(playerManager.jumpLoopState.animationName, 0.25f);
            }
        }

        //普攻连击检测
        else if (playerManager.playerInputManager.Mouse0DownInput())
        {
            //idle/walk -> combo_1_1
            ChangeState(playerManager.combo_1_1State);
            playerManager.animator.CrossFadeInFixedTime(playerManager.combo_1_1State.animationName, 0.2f);
        }
        //powerAttack输入检测: groundState -> powerAttackStart
        else if (playerManager.playerInputManager.EDownInput())
        {
            ChangeState(playerManager.powerAttackStartState);
            playerManager.animator.CrossFadeInFixedTime(playerManager.powerAttackStartState.animationName, 0.1f);
        }
    }
}
