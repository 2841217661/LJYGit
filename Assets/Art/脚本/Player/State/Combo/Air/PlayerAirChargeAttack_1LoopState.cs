using UnityEngine;

public class PlayerAirChargeAttack_1LoopState : PlayerAirComboState
{
    private Vector3 targetDir;
    public bool isLeftDash;  //ÊÇ·ñÎª×óÐîÁ¦
    public PlayerAirChargeAttack_1LoopState(PlayerManager _playerManager, string _animationName, bool _useRootMotionPart) : base(_playerManager, _animationName, _useRootMotionPart)
    {
    }

    public override void Enter()
    {
        base.Enter();

        targetDir = playerManager.playerCamera.transform.forward;

    }

    public override void Exit()
    {
        base.Exit();

    }

    public override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ChangeState(playerManager.idleState);
            playerManager.animator.CrossFadeInFixedTime(playerManager.idleState.animationName, 0.2f);
        }

        HandleMovement();

        //×óÐî
        if (isLeftDash)
        {
            if (playerManager.GroundCheck())
            {
                ChangeState(playerManager.airChargeAttack_1EndState);
                playerManager.airChargeAttack_1EndState.isLeftDash = true;
                playerManager.animator.CrossFadeInFixedTime(PlayerAnimationName.AirChargeAttack_1_1End, 0.2f);
            }
        }
        //ÓÒÐî
        else
        {
            if (playerManager.GroundCheck())
            {
                ChangeState(playerManager.airChargeAttack_1EndState);
                playerManager.airChargeAttack_1EndState.isLeftDash = false;
                playerManager.animator.CrossFadeInFixedTime(PlayerAnimationName.AirChargeAttack_1_2End, 0.2f);
            }
        }
    }

    private void HandleMovement()
    {
        playerManager.characterController.Move(targetDir * 30f * Time.deltaTime);
    }
}
