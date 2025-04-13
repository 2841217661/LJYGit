using UnityEngine;

public class PlayerIdleState : PlayerLocomotionState
{
    public PlayerIdleState(PlayerManager _playerManager, string _animationName, bool _useRootMotionPart) : base(_playerManager, _animationName, _useRootMotionPart)
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

        if(playerManager.playerInputManager.moveRawAmount != 0)
        {
            //idle -> run
            if (playerManager.playerInputManager.runInput)
            {
                ChangeState(playerManager.runState);
                playerManager.animator.CrossFadeInFixedTime(playerManager.runState.animationName, 0.2f);
            }
            //idle -> walk
            else
            {
                ChangeState(playerManager.walkState);
                playerManager.animator.CrossFadeInFixedTime(playerManager.walkState.animationName, 0.2f);
            }
        }

        ////idle -> jumpStart
        //if (playerManager.playerInputManager.JumpInput())
        //{
        //    ChangeState(playerManager.jumpStartState);
        //    playerManager.animator.CrossFadeInFixedTime(playerManager.jumpStartState.animationName, 0.2f);
        //    playerManager.jumpStartState.jumpMoveSpeed = playerManager.jumpWalkMoveSpeed;
        //}
    }
}
