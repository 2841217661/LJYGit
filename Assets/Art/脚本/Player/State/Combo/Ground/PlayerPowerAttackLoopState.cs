using UnityEngine;

public class PlayerPowerAttackLoopState : PlayerState
{
    private float duractionTime = 1f;
    private float duractionTimer;
    public PlayerPowerAttackLoopState(PlayerManager _playerManager, string _animationName, bool _useRootMotionPart) : base(_playerManager, _animationName, _useRootMotionPart)
    {
    }

    public override void Enter()
    {
        base.Enter();

        duractionTimer = 0f;
    }

    public override void Exit()
    {
        base.Exit();

    }

    public override void Update()
    {
        base.Update();

        duractionTimer += Time.deltaTime;

        if(duractionTimer > duractionTime)
        {
            ChangeState(playerManager.powerAttackEndState);
            playerManager.animator.CrossFade(playerManager.powerAttackEndState.animationName, 0.1f);
        }
    }
}
