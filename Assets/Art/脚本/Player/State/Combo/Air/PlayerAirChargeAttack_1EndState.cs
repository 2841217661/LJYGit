using UnityEngine;

public class PlayerAirChargeAttack_1EndState : PlayerAirComboState
{
    private float moveSpeed = 5f;

    public bool isLeftDash;
    public PlayerAirChargeAttack_1EndState(PlayerManager _playerManager, string _animationName, bool _useRootMotionPart) : base(_playerManager, _animationName, _useRootMotionPart)
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
        //处理移动：在落地时从loop -> end（渡入）动画过渡中需要施加一点移动，提升手感
        if (playerManager.animator.IsInTransition(0))
        {
            if (isLeftDash)
            {
                if (!stateInfo.IsName(PlayerAnimationName.AirChargeAttack_1_1End)) //渡入
                {
                    playerManager.characterController.Move(playerManager.transform.forward * Time.deltaTime * moveSpeed);

                }
                //当前处于渡出状态： //this -> idle
                else if (stateInfo.IsName(PlayerAnimationName.AirChargeAttack_1_1End)) //渡出
                {
                    ChangeState(playerManager.idleState);
                    playerManager.animator.CrossFadeInFixedTime(playerManager.idleState.animationName, 0.1f);
                }
            }
            else
            {
                if (!stateInfo.IsName(PlayerAnimationName.AirChargeAttack_1_2End)) //渡入
                {
                    playerManager.characterController.Move(playerManager.transform.forward * Time.deltaTime * moveSpeed);

                }
                //当前处于渡出状态： //this -> idle
                else if (stateInfo.IsName(PlayerAnimationName.AirChargeAttack_1_2End)) //渡出
                {
                    ChangeState(playerManager.idleState);
                    playerManager.animator.CrossFadeInFixedTime(playerManager.idleState.animationName, 0.1f);
                }
            }

        }
    }
}
