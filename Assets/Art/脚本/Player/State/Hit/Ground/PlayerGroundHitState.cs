using System;
using UnityEngine;

public class PlayerGroundHitState : PlayerGroundState
{
    public EnumManager.PlayerDamageType damageType;
    public Vector3 hitDir;

    public Action action_playHitSound; //�ܻ�����ί��
    public PlayerGroundHitState(PlayerManager _playerManager, string _animationName, bool _useRootMotionPart) : base(_playerManager, _animationName, _useRootMotionPart)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //�����ܻ�����
    }

    public override void Exit()
    {
        base.Exit();

        //�˳�ʱ����ܻ�ö��
        damageType = EnumManager.PlayerDamageType.None;
        hitDir = Vector3.zero;

        //�˳�ʱע��ί��
        action_playHitSound -= PlayHitSound;
    }

    public override void Update()
    {
        base.Update();

        action_playHitSound?.Invoke();

        AnimatorStateInfo stateInfo = playerManager.animator.GetCurrentAnimatorStateInfo(0);
        AnimatorStateInfo nextStateInfo = playerManager.animator.GetNextAnimatorStateInfo(0);

        //this -> idle(����hit��̬����idle�ڼ�)
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
                // ��λ��
                Debug.Log("�޷������ܻ�����λ��...");
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
                Debug.LogError("����,�޸�����...");
                break;
        }
    }

    private void HandleMovement(AnimationCurve _curve, float _stateInfoNormalizedTime)
    {
        float moveAmount = _curve.Evaluate(_stateInfoNormalizedTime);

        Vector3 moveVector = hitDir * moveAmount * playerManager.knockbackSpeedMul * Time.deltaTime;

        // ʹ�� CharacterController ���ƶ�
        playerManager.characterController.Move(moveVector);
    }

    public void PlayHitSound()
    {
        switch(damageType)
        {
            case EnumManager.PlayerDamageType.None:
                Debug.Log("������,���Ქ���ܻ�����");
                break;
            case EnumManager.PlayerDamageType.Light:
                //���ʲ����յ����������
                playerManager.PlayLightHitSound();
                break;
            case EnumManager.PlayerDamageType.Medium:
                //���ʲ����յ��л�������
                playerManager.PlayMediumHitSound();
                break;
            case EnumManager.PlayerDamageType.Heavy:
                //���ʲ����յ��ػ�������
                playerManager.PlayHeavyHitSound();
                break;
            default:
                Debug.LogError("���ʹ������û��֧�ֵ�����");
                break;
        }

        action_playHitSound -= PlayHitSound; //ע���¼�
    }
}
