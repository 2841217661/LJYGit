using UnityEngine;

public class PlayerJumpStartState : PlayerJumpState
{
    private bool addJumpForce;
    public PlayerJumpStartState(PlayerManager _playerManager, string _animationName, bool _useRootMotionPart) : base(_playerManager, _animationName, _useRootMotionPart)
    {
    }

    public override void Enter()
    {
        base.Enter();

        addJumpForce = false;

        //在地面姿态进行跳跃时，根据idle/walk和run/sprint使用不同的起跳动画
        if(jumpMoveSpeed == playerManager.jumpWalkMoveSpeed)
        {
            playerManager.animator.CrossFadeInFixedTime(PlayerAnimationName.JumpIdleStart, 0.2f);
        }
        else if(jumpMoveSpeed == playerManager.jumpRunMoveSpeed || jumpMoveSpeed == playerManager.jumpSprintMoveSpeed)
        {
            playerManager.animator.CrossFadeInFixedTime(PlayerAnimationName.JumpForwordStart, 0.2f);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        AnimatorStateInfo stateInfo = playerManager.animator.GetCurrentAnimatorStateInfo(0);


        if (stateInfo.IsTag(animationName))
        {
            //当前标签为jumpStart且正在过渡(即正在转换到jumpLoop)
            if (playerManager.animator.IsInTransition(0))
            {
                //jumpStart -> jumpLoop
                ChangeState(playerManager.jumpLoopState);
            }
            else //没有过渡，说明正在播放该动画
            {
                //达到25%施加一次向上跳跃的力
                if (!addJumpForce && stateInfo.normalizedTime > 0.25f)
                {
                    //计算向上跳跃的高度
                    playerManager.yVelocity.y = Mathf.Sqrt(playerManager.jumpHeight * -2f * playerManager.gravity);
                    addJumpForce = true;
                }
            }
        }
    }
}
