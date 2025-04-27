using UnityEngine;

public class PlayerSprintState : PlayerLocomotionState
{
    private float castTime = 1f; //蓄力时间
    private float castTimer; //蓄力时间计时器

    private float footstepTimer;
    private float sprintStepInterval = 0.25f;
    private bool isFoot_R;
    public PlayerSprintState(PlayerManager _playerManager, string _animationName, bool _useRootMotionPart) : base(_playerManager, _animationName, _useRootMotionPart)
    {
    }

    public override void Enter()
    {
        base.Enter();

        castTimer = 0f;

        footstepTimer = 0f;
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

        if (footstepTimer >= sprintStepInterval)
        {
            footstepTimer = 0f;

            if(isFoot_R)
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

        castTimer += Time.deltaTime;

        //sprint -> sprintStop
        if (playerManager.playerInputManager.moveRawAmount == 0)
        {
            ChangeState(playerManager.sprintStopState);
            playerManager.animator.CrossFade(playerManager.sprintStopState.animationName, 0.2f);
        }

        //sprint -> sprintAttack_1
        //在冲刺姿态下按下普通键，进行一次冲刺攻击;前提：需要在维持冲刺状态一定时间
        if (castTimer > castTime)
        {
            if (playerManager.playerInputManager.Mouse0DownInput())
            {
                ChangeState(playerManager.sprintAttackState);
                playerManager.animator.CrossFade(PlayerAnimationName.SprintAttack_1, 0.1f);
            }
            else if (playerManager.playerInputManager.Mouse1DownInput())
            {
                ChangeState(playerManager.sprintAttackState);
                playerManager.animator.CrossFade(PlayerAnimationName.SprintAttack_2, 0.1f);
            }
        }
    }
}
