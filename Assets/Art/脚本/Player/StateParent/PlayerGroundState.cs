using UnityEngine;

public class PlayerGroundState : PlayerState
{
    public PlayerGroundState(PlayerManager _playerManager, string _animationName, bool _useRootMotionPart) : base(_playerManager, _animationName, _useRootMotionPart)
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

        //跳跃输入检测
        //根据当前的姿态idle,walk,run,sprint决定jump姿态的移动速度
        if (playerManager.playerInputManager.JumpInput())
        {
            //idle,walkStop,runStop,sprintStop
            if (playerManager.currentState is PlayerIdleState || playerManager.currentState is PlayerWalkState)
            {
                playerManager.jumpStartState.jumpMoveSpeed = playerManager.jumpWalkMoveSpeed;
                playerManager.jumpLoopState.jumpMoveSpeed = playerManager.jumpWalkMoveSpeed;
            }
            else if (playerManager.currentState == playerManager.runState)
            {
                playerManager.jumpStartState.jumpMoveSpeed = playerManager.jumpRunMoveSpeed;
                playerManager.jumpLoopState.jumpMoveSpeed = playerManager.jumpRunMoveSpeed;
            }
            else if (playerManager.currentState == playerManager.sprintState)
            {
                playerManager.jumpStartState.jumpMoveSpeed = playerManager.jumpSprintMoveSpeed;
                playerManager.jumpLoopState.jumpMoveSpeed = playerManager.jumpSprintMoveSpeed;
            }

            ChangeState(playerManager.jumpStartState);
            return;
        }

        //地面到空中（下落）检测
        else if (!playerManager.isGrounded)
        {
            //ground -> jumpLoop
            if(playerManager.inAirTimer > 0.1f)
            {
                ChangeState(playerManager.jumpLoopState);
                playerManager.animator.CrossFadeInFixedTime(playerManager.jumpLoopState.animationName, 0.25f);
                return;
            }
        }

        //普攻连击检测
        else if (playerManager.playerInputManager.Mouse0DownInput())
        {
            //有一个问题，当玩家在第一下攻击后并按住移动键，在攻击结束时应当切换到run(假设玩家当前是使用的run进行移动)
            //但是玩家就是在从combo_1 -> run这个过渡过程中疯狂按鼠标左键，会导致combo_1 -> combo_1的过渡，
            //与combo_1判断逻辑发生冲突，需要加一个限制
            AnimatorStateInfo stateInfo = playerManager.animator.GetCurrentAnimatorStateInfo(0);
            if (!stateInfo.IsName(playerManager.combo_1_1State.animationName))
            {
                //idle/walk/run -> combo_1_1
                ChangeState(playerManager.combo_1_1State);
                playerManager.animator.CrossFadeInFixedTime(playerManager.combo_1_1State.animationName, 0.2f);
                return;
            }
        }
        //powerAttack输入检测: groundState -> powerAttackStart
        else if (playerManager.playerInputManager.EDownInput())
        {
            ChangeState(playerManager.powerAttackStartState);
            playerManager.animator.CrossFadeInFixedTime(playerManager.powerAttackStartState.animationName, 0.1f);
            return;
        }
    }
}
