using UnityEngine;

//在这个大风车姿态下，启动重力，以一定的速度向前移动，不能进行移动和旋转输入
public class PlayerAirChargeAttack_2StartState : PlayerAirComboState
{
    public PlayerAirChargeAttack_2StartState(PlayerManager _playerManager, string _animationName, bool _useRootMotionPart) : base(_playerManager, _animationName, _useRootMotionPart)
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

        AnimatorStateInfo stateInfo = playerManager.animator.GetCurrentAnimatorStateInfo(0);
        //正在过渡到loop,则切换姿态: this -> loop
        if (stateInfo.IsName(animationName) && playerManager.animator.IsInTransition(0))
        {
            ChangeState(playerManager.airChargeAttack_2LoopState);
        }
        
    }
}
