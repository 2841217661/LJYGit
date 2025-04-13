using Unity.VisualScripting;
using UnityEngine;

public class PlayerWalkState : PlayerLocomotionState
{
    private float timer; 
    public PlayerWalkState(PlayerManager _playerManager, string _animationName, bool _useRootMotionPart) : base(_playerManager, _animationName, _useRootMotionPart)
    {
    }

    public override void Enter()
    {
        base.Enter();

        timer = 0;
    }

    public override void Exit()
    {
        base.Exit();

    }

    public override void Update()
    {
        base.Update();

        timer += Time.deltaTime;

        if (playerManager.playerInputManager.runInput)
        {
            ChangeState(playerManager.runState);
            playerManager.animator.CrossFade(playerManager.runState.animationName, 0.2f);
        }

        if (playerManager.playerInputManager.moveRawAmount == 0)
        {

            if(timer > 0.3f)
            {
                ChangeState(playerManager.walkStopState);
                playerManager.animator.CrossFadeInFixedTime(playerManager.walkStopState.animationName, 0.2f);
            }
            else
            {
                ChangeState(playerManager.idleState);
                playerManager.animator.CrossFadeInFixedTime(playerManager.idleState.animationName, 0.2f);
            }
        }
    }
}
