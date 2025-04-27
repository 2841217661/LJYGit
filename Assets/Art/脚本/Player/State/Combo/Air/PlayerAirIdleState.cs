using UnityEngine;

public class PlayerAirIdleState : PlayerAirState
{
    private float airIdleTimer;
    private float airIdleTime = 2f;

    private float chargeAttackTimer;
    private float chargeAttackTime = 0.2f;
    public PlayerAirIdleState(PlayerManager _playerManager, string _animationName, bool _useRootMotionPart) : base(_playerManager, _animationName, _useRootMotionPart)
    {
    }

    public override void Enter()
    {
        base.Enter();

        airIdleTimer = 0f;
        chargeAttackTimer = 0f;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        //�ڸ���״̬�¿ɽ����ƶ����ƶ��ٶȺ�jumpWalkMoveSpeed��ͬ����ת�ٶ�ΪjumpRotationSpeed
        HandleMove();
        HandleRotate();

        airIdleTimer += Time.deltaTime;

        // this -> idle
        if (airIdleTimer > airIdleTime)
        {
            ChangeState(playerManager.idleState);
        }
        else if(playerManager.playerInputManager.CDownInput()) //this -> idle:��C�����Ϳ���̬
        {
            ChangeState(playerManager.idleState);
        }
        else if (playerManager.playerInputManager.DodgeInput()) // this -> airDodge
        {
            
            //�жϵ�ǰ�Ƿ������ڲ��ŵ�ǰ��̬����:��ֹairDodge -> airIdle���ڹ����У�Ȼ���ٴν���airDodge������bug
            if (playerManager.animator.GetCurrentAnimatorStateInfo(0).IsName(animationName))
            {
                ChangeState(playerManager.airDodgeState);


            }
        }
        else if (playerManager.playerInputManager.Mouse0UpInput()) //this -> airCombo_1_1
        {
            ChangeState(playerManager.airCombo_1_1State);
            playerManager.animator.CrossFade(playerManager.airCombo_1_1State.animationName, 0.2f);
        }
        else if (playerManager.playerInputManager.Mouse0Input()) //this -> chargeAirAttack_1
        {
            chargeAttackTimer += Time.deltaTime;
            if(chargeAttackTimer > chargeAttackTime)
            {
                ChangeState(playerManager.airChargeAttack_1StartState);
                playerManager.airChargeAttack_1StartState.isLeftDash = true;
                playerManager.animator.CrossFade(PlayerAnimationName.AirChargeAttack_1_1Start, 0.2f);
            }
        }
        else if (playerManager.playerInputManager.Mouse1Input()) //this -> chargeAirAttack_2
        {
            chargeAttackTimer += Time.deltaTime;
            if (chargeAttackTimer > chargeAttackTime)
            {
                ChangeState(playerManager.airChargeAttack_1StartState);
                playerManager.airChargeAttack_1StartState.isLeftDash = false;
                playerManager.animator.CrossFade(PlayerAnimationName.AirChargeAttack_1_2Start, 0.2f);
            }
        }
        else
        {
            chargeAttackTimer = 0f;
        }

    }

    private void HandleMove()
    {
        Vector3 moveDirection;
        moveDirection = playerManager.playerCamera.transform.forward * playerManager.playerInputManager.movementInput.y
                + playerManager.playerCamera.transform.right * playerManager.playerInputManager.movementInput.x;
        moveDirection.Normalize(); //��һ�����õ���������
        moveDirection.y = 0; //������y����ƶ�����ֻ����ƽ�棨���棩�ƶ�

        playerManager.characterController.Move(playerManager.transform.forward * playerManager.jumpWalkMoveSpeed * playerManager.playerInputManager.moveAmount * Time.deltaTime);
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

        float rotationSpeed = playerManager.jumpRotationSpeed;

        // ƽ����ת��Ŀ��
        Quaternion targetRotation = Quaternion.Slerp(playerManager.transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
        playerManager.transform.rotation = targetRotation;
    }
}
