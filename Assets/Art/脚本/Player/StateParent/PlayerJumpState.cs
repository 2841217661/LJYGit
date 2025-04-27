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

        //jumpStop�����ƶ�����ת
        if(playerManager.currentState == playerManager.jumpStopState)
        {
            return;
        }

        //�ƶ�����
        HandleMove();

        //��ת����
        HandleRotate();
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

        // ƽ����ת��Ŀ��
        Quaternion targetRotation = Quaternion.Slerp(playerManager.transform.rotation, newRotation, playerManager.jumpRotationSpeed * Time.deltaTime);
        playerManager.transform.rotation = targetRotation;

    }

    private void HandleMove()
    {
        Vector3 moveDirection;
        moveDirection = playerManager.playerCamera.transform.forward * playerManager.playerInputManager.movementInput.y
                + playerManager.playerCamera.transform.right * playerManager.playerInputManager.movementInput.x;
        moveDirection.Normalize(); //��һ�����õ���������
        moveDirection.y = 0; //������y����ƶ�����ֻ����ƽ�棨���棩�ƶ�

        playerManager.characterController.Move(moveDirection * jumpMoveSpeed * playerManager.playerInputManager.moveAmount * Time.deltaTime);
    }

}
