using UnityEngine;

public class PlayerAirDodgeState : PlayerAirState
{
    public PlayerAirDodgeState(PlayerManager _playerManager, string _animationName, bool _useRootMotionPart) : base(_playerManager, _animationName, _useRootMotionPart)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //根据当前是否有移动输入进行不同动画
        if (playerManager.playerInputManager.moveRawAmount == 0)
        {
            playerManager.animator.CrossFade(PlayerAnimationName.AirDodge_B, 0.2f);
        }
        else
        {
            playerManager.animator.CrossFade(PlayerAnimationName.AirDodge_F, 0.2f);
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
        //检查当前是否处于渡出状态
        if (stateInfo.IsTag(animationName) && playerManager.animator.IsInTransition(0))
        {
            // this -> airIdle
            ChangeState(playerManager.airIdleState);
        }
    }
}
