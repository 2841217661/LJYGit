using System;
using UnityEngine;

public class PlayerGroundHitState : PlayerGroundState
{
    public EnumManager.PlayerDamageType damageType;
    public Vector3 hitDir;

    public Action action_playHitSound; //受击语音委托
    public PlayerGroundHitState(PlayerManager _playerManager, string _animationName, bool _useRootMotionPart) : base(_playerManager, _animationName, _useRootMotionPart)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //播放受击音乐
    }

    public override void Exit()
    {
        base.Exit();

        //退出时清空受击枚举
        damageType = EnumManager.PlayerDamageType.None;
        hitDir = Vector3.zero;

        //退出时注销委托
        action_playHitSound -= PlayHitSound;
    }

    public override void Update()
    {
        base.Update();

        action_playHitSound?.Invoke();

        AnimatorStateInfo stateInfo = playerManager.animator.GetCurrentAnimatorStateInfo(0);
        AnimatorStateInfo nextStateInfo = playerManager.animator.GetNextAnimatorStateInfo(0);

        //this -> idle(处于hit姿态过渡idle期间)
        if (stateInfo.IsTag("Hit") && nextStateInfo.IsName(playerManager.idleState.animationName))
        {
            ChangeState(playerManager.idleState);
        }
        else
        {
            SelectMoveAction(stateInfo.normalizedTime);
        }
    }

    private void SelectMoveAction(float _stateInfoNormalizedTime)
    {
        AnimatorStateInfo stateInfo = playerManager.animator.GetCurrentAnimatorStateInfo(0);
        AnimatorStateInfo nextStateInfo = playerManager.animator.GetNextAnimatorStateInfo(0);
        if (stateInfo.IsName(PlayerAnimationName.Get_Up) || nextStateInfo.IsName(PlayerAnimationName.Get_Up)) return;

        switch (damageType)
        {
            case EnumManager.PlayerDamageType.None:
                // 无位移
                Debug.Log("无法进行受击后退位移...");
                break;

            case EnumManager.PlayerDamageType.Light:
                HandleMovement(playerManager.damageLightBackMoveCurve, _stateInfoNormalizedTime);
                break;

            case EnumManager.PlayerDamageType.Medium:
                HandleMovement(playerManager.damageMediumBackMoveCurve, _stateInfoNormalizedTime);
                break;

            case EnumManager.PlayerDamageType.Heavy:
                HandleMovement(playerManager.damageHeavyBackMoveCurve, _stateInfoNormalizedTime);
                break;
            default:
                Debug.LogError("错误,无该类型...");
                break;
        }
    }

    private void HandleMovement(AnimationCurve _curve, float _stateInfoNormalizedTime)
    {
        float moveAmount = _curve.Evaluate(_stateInfoNormalizedTime);

        Vector3 moveVector = hitDir * moveAmount * playerManager.knockbackSpeedMul * Time.deltaTime;

        // 使用 CharacterController 来移动
        playerManager.characterController.Move(moveVector);
    }

    public void PlayHitSound()
    {
        switch(damageType)
        {
            case EnumManager.PlayerDamageType.None:
                Debug.Log("无类型,不会播放受击语音");
                break;
            case EnumManager.PlayerDamageType.Light:
                //概率播放收到轻击的语音
                playerManager.PlayLightHitSound();
                break;
            case EnumManager.PlayerDamageType.Medium:
                //概率播放收到中击的语音
                playerManager.PlayMediumHitSound();
                break;
            case EnumManager.PlayerDamageType.Heavy:
                //概率播放收到重击的语音
                playerManager.PlayHeavyHitSound();
                break;
            default:
                Debug.LogError("类型错误或者没有支持的类型");
                break;
        }

        action_playHitSound -= PlayHitSound; //注销事件
    }
}
