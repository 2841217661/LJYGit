using UnityEngine;

public class PlayerAirChargeAttack_1StartState : PlayerAirComboState
{
    public bool isLeftDash;  //是否为左蓄力
    private float dashTime = 0.5f; //蓄力可释放的时间
    private float dashTimer;
    private bool canDash; //是否可释放
    public PlayerAirChargeAttack_1StartState(PlayerManager _playerManager, string _animationName, bool _useRootMotionPart) : base(_playerManager, _animationName, _useRootMotionPart)
    {
    }

    public override void Enter()
    {
        base.Enter();

        dashTimer = 0f;
        canDash = false;
    }

    public override void Exit()
    {
        base.Exit();

    }

    public override void Update()
    {
        base.Update();

        //蓄力过程中Player可随鼠标进行旋转
        HandleTheRotate();

        //左蓄
        if (isLeftDash)
        {
            if (playerManager.playerInputManager.Mouse0Input()) //蓄力
            {
                dashTimer += Time.deltaTime;
                if(dashTimer > dashTime) //可进行释放
                {
                    canDash = true;
                }
            }
            else if (playerManager.playerInputManager.Mouse0UpInput()) //释放
            {
                //判断是否可以进行释放
                if (canDash) //释放：this -> loop
                {
                    ChangeState(playerManager.airChargeAttack_1LoopState);
                    playerManager.airChargeAttack_1LoopState.isLeftDash = true;
                }
                //不可进行释放：this -> airIdle
                else
                {
                    ChangeState(playerManager.airIdleState);
                    playerManager.animator.CrossFade(playerManager.airIdleState.animationName, 0.2f);
                }
            }
        }
        //右蓄
        else
        {
            if (playerManager.playerInputManager.Mouse1Input()) //蓄力
            {
                dashTimer += Time.deltaTime;
                if (dashTimer > dashTime) //可进行释放
                {
                    canDash = true;
                }
            }
            else if (playerManager.playerInputManager.Mouse1UpInput()) //释放
            {
                //判断是否可以进行释放
                if (canDash) //释放：this -> loop
                {
                    ChangeState(playerManager.airChargeAttack_1LoopState);
                    playerManager.airChargeAttack_1LoopState.isLeftDash = false;
                }
                //不可进行释放：this -> airIdle
                else
                {
                    ChangeState(playerManager.airIdleState);
                    playerManager.animator.CrossFade(playerManager.airIdleState.animationName, 0.2f);
                }
            }
        }

    }

    private void HandleTheRotate()
    {
        //目标方向
        Vector3 targetRotationDirection = playerManager.playerCamera.transform.forward;

        targetRotationDirection.Normalize();
        targetRotationDirection.y = 0; //只在水平面进行旋转

        // 计算目标旋转
        Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);

        float rotationSpeed = 10f;

        // 平滑旋转到目标
        Quaternion targetRotation = Quaternion.Slerp(playerManager.transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
        playerManager.transform.rotation = targetRotation;
    }
}
