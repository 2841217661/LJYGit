using UnityEngine;

//�������糵��̬�£�������������һ�����ٶ���ǰ�ƶ������ܽ����ƶ�����ת����
public class PlayerAirChargeAttack_2StartState : PlayerAirComboState
{
    public PlayerAirChargeAttack_2StartState(PlayerManager _playerManager, string _animationName, bool _useRootMotionPart) : base(_playerManager, _animationName, _useRootMotionPart)
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

        AnimatorStateInfo stateInfo = playerManager.animator.GetCurrentAnimatorStateInfo(0);
        //���ڹ��ɵ�loop,���л���̬: this -> loop
        if (stateInfo.IsName(animationName) && playerManager.animator.IsInTransition(0))
        {
            ChangeState(playerManager.airChargeAttack_2LoopState);
        }
        
    }
}
