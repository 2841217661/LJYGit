using UnityEngine;

public class PlayerGroundComboState : PlayerComboState
{

    public PlayerGroundComboState(PlayerManager _playerManager, string _animationName, bool _useRootMotionPart) : base(_playerManager, _animationName, _useRootMotionPart)
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

        //闪避优先级非常高，可以打断地面连击姿态
        if (playerManager.playerInputManager.DodgeInput())
        {
            ChangeState(playerManager.dodgeState);

            // 计算目标旋转方向
            Vector3 targetRotationDirection = playerManager.playerCamera.transform.forward * playerManager.playerInputManager.movementInput.y
                                            + playerManager.playerCamera.transform.right * playerManager.playerInputManager.movementInput.x;

            targetRotationDirection.Normalize();
            targetRotationDirection.y = 0f;

            playerManager.dodgeState.moveDir = targetRotationDirection;
        }
    }


}
