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

        //�ƶ�����
        HandleMove();

        //��ת����
        HandleRotate();


        //����������
        if (playerManager.playerInputManager.DodgeInput())
        {
            ChangeState(playerManager.dodgeState);

            // ����Ŀ����ת����
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
        // ����Ŀ����ת����
        Vector3 targetRotationDirection = playerManager.playerCamera.transform.forward * playerManager.playerInputManager.movementInput.y
                                        + playerManager.playerCamera.transform.right * playerManager.playerInputManager.movementInput.x;

        targetRotationDirection.Normalize();
        targetRotationDirection.y = 0f;

        // û���������
        if (targetRotationDirection == Vector3.zero)
        {
            targetRotationDirection = playerManager.transform.forward;
        }

        // ����Ŀ����ת
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

        // ƽ����ת��Ŀ��
        Quaternion targetRotation = Quaternion.Slerp(playerManager.transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
        playerManager.transform.rotation = targetRotation;

        // ����Ŀ�귽�����ɫ��ǰ����ļнǲ����
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
        moveDirection.Normalize(); //��һ�����õ���������
        moveDirection.y = 0; //������y����ƶ�����ֻ����ƽ�棨���棩�ƶ�

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
