using UnityEngine;

public class PlayerChargeAttack_1State : PlayerAirComboState
{
    public PlayerChargeAttack_1State(PlayerManager _playerManager, string _animationName, bool _useRootMotionPart) : base(_playerManager, _animationName, _useRootMotionPart)
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
        if (stateInfo.IsName(animationName))
        {
            if (playerManager.animator.IsInTransition(0))
            {
                //chargeAttack_1 -> airIdle
                ChangeState(playerManager.airIdleState);
            }
        }
    }
}
