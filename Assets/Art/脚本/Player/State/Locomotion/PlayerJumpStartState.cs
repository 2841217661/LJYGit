using UnityEngine;

public class PlayerJumpStartState : PlayerJumpState
{
    private bool addJumpForce;
    public PlayerJumpStartState(PlayerManager _playerManager, string _animationName, bool _useRootMotionPart) : base(_playerManager, _animationName, _useRootMotionPart)
    {
    }

    public override void Enter()
    {
        base.Enter();

        addJumpForce = false;

        //�ڵ�����̬������Ծʱ������idle/walk��run/sprintʹ�ò�ͬ����������
        if(jumpMoveSpeed == playerManager.jumpWalkMoveSpeed)
        {
            playerManager.animator.CrossFadeInFixedTime(PlayerAnimationName.JumpIdleStart, 0.2f);
        }
        else if(jumpMoveSpeed == playerManager.jumpRunMoveSpeed || jumpMoveSpeed == playerManager.jumpSprintMoveSpeed)
        {
            playerManager.animator.CrossFadeInFixedTime(PlayerAnimationName.JumpForwordStart, 0.2f);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        AnimatorStateInfo stateInfo = playerManager.animator.GetCurrentAnimatorStateInfo(0);


        if (stateInfo.IsTag(animationName))
        {
            //��ǰ��ǩΪjumpStart�����ڹ���(������ת����jumpLoop)
            if (playerManager.animator.IsInTransition(0))
            {
                //jumpStart -> jumpLoop
                ChangeState(playerManager.jumpLoopState);
            }
            else //û�й��ɣ�˵�����ڲ��Ÿö���
            {
                //�ﵽ25%ʩ��һ��������Ծ����
                if (!addJumpForce && stateInfo.normalizedTime > 0.25f)
                {
                    //����������Ծ�ĸ߶�
                    playerManager.yVelocity.y = Mathf.Sqrt(playerManager.jumpHeight * -2f * playerManager.gravity);
                    addJumpForce = true;
                }
            }
        }
    }
}
