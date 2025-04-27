using UnityEngine;

public class PlayerWalkStopState : PlayerIdleState
{
    public PlayerWalkStopState(PlayerManager _playerManager, string _animationName, bool _useRootMotionPart) : base(_playerManager, _animationName, _useRootMotionPart)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (playerManager.animator.GetCurrentAnimatorStateInfo(0).IsName(animationName))
        {
            //walkStop -> idle
            //�����ǰ������"WalkStop"���Ҵ��ڹ���״̬����˵���ö����Ѿ�����WalkStop�л�ΪIdle
            if (playerManager.animator.IsInTransition(0))
            {
                ChangeState(playerManager.idleState);
                playerManager.animator.CrossFadeInFixedTime(playerManager.idleState.animationName, 0.2f);
            }

        }
    }
}
