using UnityEngine;

public class PlayerRunState : PlayerLocomotionState
{
    private float timer;

    private float footstepTimer;
    private float runStepInterval = 0.35f;
    private bool isFoot_R;
    public PlayerRunState(PlayerManager _playerManager, string _animationName, bool _useRootMotionPart) : base(_playerManager, _animationName, _useRootMotionPart)
    {
    }

    public override void Enter()
    {
        base.Enter();

        timer = 0f;
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

        //���Žű���
        footstepTimer += Time.deltaTime;

        if (footstepTimer >= runStepInterval)
        {
            footstepTimer = 0f;

            if (isFoot_R)
            {
                //����һ���ҽŽű���
                AudioManager.Instance.PlaySound(AudioPathConfi.SFX_Foot_R, AudioType.SFX, playerManager.transform.position);
                isFoot_R = false;
            }
            else
            {
                //����һ����Žű���
                AudioManager.Instance.PlaySound(AudioPathConfi.SFX_Foot_L, AudioType.SFX, playerManager.transform.position);
                isFoot_R = true;
            }
        }

        timer += Time.deltaTime;

        //��run��̬�л����ߣ�ֱ��ת��Ϊwalk run -> walk
        if (!playerManager.playerInputManager.runInput)
        {
            ChangeState(playerManager.walkState);
            playerManager.animator.CrossFade(playerManager.walkState.animationName, 0.2f);
        }

        //run -> runStop; run -> idle
        if (playerManager.playerInputManager.moveRawAmount == 0)
        {
            //����һ��ʱ����ܼ�ͣ
            if (timer > 0.3f)
            {
                ChangeState(playerManager.runStopState);
                playerManager.animator.CrossFadeInFixedTime(playerManager.runStopState.animationName, 0.2f);
            }
            else //����ֱ�ӽ���idle
            {
                ChangeState(playerManager.idleState);
                playerManager.animator.CrossFadeInFixedTime(playerManager.idleState.animationName, 0.2f);
            }
        }
    }
}
