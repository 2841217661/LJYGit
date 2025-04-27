using UnityEngine;

public class PlayerDodgeState : PlayerState
{
    //假设player面向前方，玩家此时按下s和shift，此时的逻辑：player播放闪避动画，但由于角色需要旋转180°，因此在闪避中添加旋转逻辑
    public Vector3 moveDir;


    public PlayerDodgeState(PlayerManager _playerManager, string _animationName, bool _useRootMotionPart) : base(_playerManager, _animationName, _useRootMotionPart)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //播放一次闪避音效
        AudioManager.Instance.PlaySound(AudioPathConfi.SFX_Dodge, AudioType.SFX, playerManager.transform.position);
        //概率播放一次语音
        playerManager.PlayPlayerDodgeSound();

        if (playerManager.playerInputManager.moveRawAmount == 0)
        {
            playerManager.animator.CrossFadeInFixedTime(PlayerAnimationName.DodgeBack, 0.2f);
        }
        else
        {
            playerManager.animator.CrossFadeInFixedTime(PlayerAnimationName.DodgeForword, 0.2f);
        }
    }

    public override void Exit()
    {
        base.Exit();

        moveDir = Vector3.zero;
    }

    public override void Update()
    {
        base.Update();

        //当前处于对应动画
        if (playerManager.animator.GetCurrentAnimatorStateInfo(0).IsTag(animationName))
        {
            //dodge -> idle
            if (playerManager.animator.IsInTransition(0))
            {
                ChangeState(playerManager.idleState);
                playerManager.animator.CrossFadeInFixedTime(playerManager.idleState.animationName, 0.25f);
            }
        }

        //应用旋转
        if (moveDir != Vector3.zero)
        {
            // 计算目标旋转
            Quaternion newRotation = Quaternion.LookRotation(moveDir);
            // 平滑旋转到目标
            Quaternion targetRotation = Quaternion.Slerp(playerManager.transform.rotation, newRotation, playerManager.dodgeRotateSpeed * Time.deltaTime);
            playerManager.transform.rotation = targetRotation;
        }

        if(playerManager.animator.GetCurrentAnimatorStateInfo(0).IsTag(animationName) && playerManager.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.35f)
        {
            //冲刺动画达到一定时间段仍然有移动输入，则切换为sprint姿态
            if(playerManager.playerInputManager.moveRawAmount != 0)
            {
                ChangeState(playerManager.sprintState);
                playerManager.animator.CrossFade(playerManager.sprintState.animationName, 0.2f);
            }
        }
    }
}
