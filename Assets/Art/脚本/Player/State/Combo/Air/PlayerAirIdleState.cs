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

        //在浮空状态下可进行移动，移动速度和jumpWalkMoveSpeed相同，旋转速度为jumpRotationSpeed
        HandleMove();
        HandleRotate();

        airIdleTimer += Time.deltaTime;

        // this -> idle
        if (airIdleTimer > airIdleTime)
        {
            ChangeState(playerManager.idleState);
        }
        else if(playerManager.playerInputManager.CDownInput()) //this -> idle:按C结束滞空姿态
        {
            ChangeState(playerManager.idleState);
        }
        else if (playerManager.playerInputManager.DodgeInput()) // this -> airDodge
        {
            
            //判断当前是否处于正在播放当前姿态动画:防止airDodge -> airIdle正在过渡中，然后再次进入airDodge产生的bug
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
        moveDirection.Normalize(); //归一化，得到方向向量
        moveDirection.y = 0; //忽略在y轴的移动，及只能在平面（地面）移动

        playerManager.characterController.Move(playerManager.transform.forward * playerManager.jumpWalkMoveSpeed * playerManager.playerInputManager.moveAmount * Time.deltaTime);
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

        float rotationSpeed = playerManager.jumpRotationSpeed;

        // 平滑旋转到目标
        Quaternion targetRotation = Quaternion.Slerp(playerManager.transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
        playerManager.transform.rotation = targetRotation;
    }
}
