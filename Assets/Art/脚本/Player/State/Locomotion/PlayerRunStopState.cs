using UnityEngine;

public class PlayerRunStopState : PlayerIdleState
{
    public PlayerRunStopState(PlayerManager _playerManager, string _animationName, bool _useRootMotionPart) : base(_playerManager, _animationName, _useRootMotionPart)
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

        if (playerManager.animator.GetCurrentAnimatorStateInfo(0).IsName(animationName))
        {
            //runStop -> idle
            //如果当前动画是"WalkStop"并且处于过渡状态，则说明该动画已经在由WalkStop切换为Idle
            if (playerManager.animator.IsInTransition(0))
            {
                ChangeState(playerManager.idleState);
                playerManager.animator.CrossFadeInFixedTime(playerManager.idleState.animationName, 0.2f);
            }
        }
    }
}
