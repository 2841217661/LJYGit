using UnityEngine;

public class PlayerLocomotionState : PlayerGroundState
{
    public float angle;
    public PlayerLocomotionState(PlayerManager _playerManager, string _animationName, bool _useRootMotionPart) : base(_playerManager, _animationName, _useRootMotionPart)
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

        //移动处理
        HandleMove();

        //旋转处理
        HandleRotate();


        //闪避输入检测
        if (playerManager.playerInputManager.DodgeInput())
        {
            ChangeState(playerManager.dodgeState);

            // 计算目标旋转方向
            Vector3 targetRotationDirection = playerManager.playerCamera.transform.forward * playerManager.playerInputManager.movementInput.y
                                            + playerManager.playerCamera.transform.right * playerManager.playerInputManager.movementInput.x;

            targetRotationDirection.Normalize();
            targetRotationDirection.y = 0f;

            playerManager.dodgeState.moveDir = targetRotationDirection;

            return;
        }
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

        float rotationSpeed = 0f;

        if (playerManager.currentState == playerManager.walkState)
        {
            rotationSpeed = playerManager.walkRotationSpeed;
        }
        else if(playerManager.currentState == playerManager.runState)
        {
            rotationSpeed = playerManager.runRotationSpeed;
        }
        else if(playerManager.currentState == playerManager.sprintState)
        {
            rotationSpeed = playerManager.sprintRotationSpeed;
        }

        // 平滑旋转到目标
        Quaternion targetRotation = Quaternion.Slerp(playerManager.transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
        playerManager.transform.rotation = targetRotation;

        // 计算目标方向与角色当前朝向的夹角并输出
        angle = Vector3.SignedAngle(playerManager.transform.forward, targetRotationDirection, Vector3.up);

        angle = angle / 180 * playerManager.turnAngleMul;
        angle = Mathf.Clamp(angle, -1, 1);
        playerManager.animator.SetFloat("Turn", angle, 0.15f,Time.deltaTime);
        playerManager.animator.SetFloat("MoveAmount", playerManager.playerInputManager.moveRawAmount, 0.1f,Time.deltaTime);

    }

    private void HandleMove()
    {
        Vector3 moveDirection;
        moveDirection = playerManager.playerCamera.transform.forward * playerManager.playerInputManager.movementInput.y
                + playerManager.playerCamera.transform.right * playerManager.playerInputManager.movementInput.x;
        moveDirection.Normalize(); //归一化，得到方向向量
        moveDirection.y = 0; //忽略在y轴的移动，及只能在平面（地面）移动

        if (playerManager.currentState == playerManager.walkState)
        {
            playerManager.characterController.Move(playerManager.transform.forward * playerManager.walkSpeed * playerManager.playerInputManager.moveAmount * Time.deltaTime);
        }
        else if (playerManager.currentState == playerManager.runState)
        {
            playerManager.characterController.Move(playerManager.transform.forward * playerManager.runSpeed * playerManager.playerInputManager.moveAmount * Time.deltaTime);
        }
        else if (playerManager.currentState == playerManager.sprintState)
        {
            playerManager.characterController.Move(playerManager.transform.forward * playerManager.sprintSpeed * playerManager.playerInputManager.moveAmount * Time.deltaTime);
        }
    }


}
