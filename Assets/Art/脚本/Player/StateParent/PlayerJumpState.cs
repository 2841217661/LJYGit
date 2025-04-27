using UnityEngine;

public class PlayerJumpState : PlayerAirState
{
    public float jumpMoveSpeed;
    public bool useGravity = true;
    public PlayerJumpState(PlayerManager _playerManager, string _animationName, bool _useRootMotionPart) : base(_playerManager, _animationName, _useRootMotionPart)
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

        playerManager.HandleGravity(useGravity);

        //jumpStop不能移动和旋转
        if(playerManager.currentState == playerManager.jumpStopState)
        {
            return;
        }

        //移动处理
        HandleMove();

        //旋转处理
        HandleRotate();
    }

    private void HandleRotate()
    {
        // 计算目标旋转方向
        Vector3 targetRotationDirection = playerManager.playerCamera.transform.forward * playerManager.playerInputManager.movementInput.y
                                        + playerManager.playerCamera.transform.right * playerManager.playerInputManager.movementInput.x;

        targetRotationDirection.Normalize();
        targetRotationDirection.y = 0f;

        // 没有相机输入
        if (targetRotationDirection == Vector3.zero)
        {
            targetRotationDirection = playerManager.transform.forward;
        }

        // 计算目标旋转
        Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);

        // 平滑旋转到目标
        Quaternion targetRotation = Quaternion.Slerp(playerManager.transform.rotation, newRotation, playerManager.jumpRotationSpeed * Time.deltaTime);
        playerManager.transform.rotation = targetRotation;

    }

    private void HandleMove()
    {
        Vector3 moveDirection;
        moveDirection = playerManager.playerCamera.transform.forward * playerManager.playerInputManager.movementInput.y
                + playerManager.playerCamera.transform.right * playerManager.playerInputManager.movementInput.x;
        moveDirection.Normalize(); //归一化，得到方向向量
        moveDirection.y = 0; //忽略在y轴的移动，及只能在平面（地面）移动

        playerManager.characterController.Move(moveDirection * jumpMoveSpeed * playerManager.playerInputManager.moveAmount * Time.deltaTime);
    }

}
