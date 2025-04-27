using UnityEngine;

public class PlayerDodgeState : PlayerState
{
    //����player����ǰ������Ҵ�ʱ����s��shift����ʱ���߼���player�������ܶ����������ڽ�ɫ��Ҫ��ת180�㣬����������������ת�߼�
    public Vector3 moveDir;


    public PlayerDodgeState(PlayerManager _playerManager, string _animationName, bool _useRootMotionPart) : base(_playerManager, _animationName, _useRootMotionPart)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //����һ��������Ч
        AudioManager.Instance.PlaySound(AudioPathConfi.SFX_Dodge, AudioType.SFX, playerManager.transform.position);
        //���ʲ���һ������
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

        //��ǰ���ڶ�Ӧ����
        if (playerManager.animator.GetCurrentAnimatorStateInfo(0).IsTag(animationName))
        {
            //dodge -> idle
            if (playerManager.animator.IsInTransition(0))
            {
                ChangeState(playerManager.idleState);
                playerManager.animator.CrossFadeInFixedTime(playerManager.idleState.animationName, 0.25f);
            }
        }

        //Ӧ����ת
        if (moveDir != Vector3.zero)
        {
            // ����Ŀ����ת
            Quaternion newRotation = Quaternion.LookRotation(moveDir);
            // ƽ����ת��Ŀ��
            Quaternion targetRotation = Quaternion.Slerp(playerManager.transform.rotation, newRotation, playerManager.dodgeRotateSpeed * Time.deltaTime);
            playerManager.transform.rotation = targetRotation;
        }

        if(playerManager.animator.GetCurrentAnimatorStateInfo(0).IsTag(animationName) && playerManager.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.35f)
        {
            //��̶����ﵽһ��ʱ�����Ȼ���ƶ����룬���л�Ϊsprint��̬
            if(playerManager.playerInputManager.moveRawAmount != 0)
            {
                ChangeState(playerManager.sprintState);
                playerManager.animator.CrossFade(playerManager.sprintState.animationName, 0.2f);
            }
        }
    }
}
