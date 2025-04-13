using UnityEngine;

public class PlayerJumpStopState : PlayerJumpState
{
    public PlayerJumpStopState(PlayerManager _playerManager, string _animationName, bool _useRootMotionPart) : base(_playerManager, _animationName, _useRootMotionPart)
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

        if (playerManager.animator.GetCurrentAnimatorStateInfo(0).IsName(animationName) && playerManager.animator.IsInTransition(0))
        {
            //jumpStop -> idle
            ChangeState(playerManager.idleState);
            playerManager.animator.CrossFadeInFixedTime(playerManager.idleState.animationName, 0.25f);
        }

    }
}
