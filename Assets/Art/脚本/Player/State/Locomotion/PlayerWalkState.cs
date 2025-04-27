using Unity.VisualScripting;
using UnityEngine;

public class PlayerWalkState : PlayerLocomotionState
{
    private float timer;

    private float footstepTimer;
    private float walkStepInterval = 0.5f;
    private bool isFoot_R;

    public PlayerWalkState(PlayerManager _playerManager, string _animationName, bool _useRootMotionPart) : base(_playerManager, _animationName, _useRootMotionPart)
    {
    }

    public override void Enter()
    {
        base.Enter();

        timer = 0;
        footstepTimer = 0;
        isFoot_R = false;
    }

    public override void Exit()
    {
        base.Exit();

    }

    public override void Update()
    {
        base.Update();

        //播放脚本声
        footstepTimer += Time.deltaTime;

        if (footstepTimer >= walkStepInterval)
        {
            footstepTimer = 0f;

            if (isFoot_R)
            {
                //播放一次右脚脚本声
                AudioManager.Instance.PlaySound(AudioPathConfi.SFX_Foot_R, AudioType.SFX, playerManager.transform.position);
                isFoot_R = false;
            }
            else
            {
                //播放一次左脚脚本声
                AudioManager.Instance.PlaySound(AudioPathConfi.SFX_Foot_L, AudioType.SFX, playerManager.transform.position);
                isFoot_R = true;
            }
        }

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
