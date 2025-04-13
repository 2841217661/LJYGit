using UnityEngine;

public class PlayerRunState : PlayerLocomotionState
{
    private float timer;
    public PlayerRunState(PlayerManager _playerManager, string _animationName, bool _useRootMotionPart) : base(_playerManager, _animationName, _useRootMotionPart)
    {
    }

    public override void Enter()
    {
        base.Enter();

        timer = 0f;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        timer += Time.deltaTime;

        //在run姿态切换行走，直接转换为walk run -> walk
        if (!playerManager.playerInputManager.runInput)
        {
            ChangeState(playerManager.walkState);
            playerManager.animator.CrossFade(playerManager.walkState.animationName, 0.2f);
        }

        //run -> runStop; run -> idle
        if (playerManager.playerInputManager.moveRawAmount == 0)
        {
            //持续一段时间才能急停
            if (timer > 0.3f)
            {
                ChangeState(playerManager.runStopState);
                playerManager.animator.CrossFadeInFixedTime(playerManager.runStopState.animationName, 0.2f);
            }
            else //否则直接进入idle
            {
                ChangeState(playerManager.idleState);
                playerManager.animator.CrossFadeInFixedTime(playerManager.idleState.animationName, 0.2f);
            }
        }
    }
}
