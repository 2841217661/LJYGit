using UnityEngine;

public class PlayerAirChargeAttack_2EndState : PlayerAirComboState
{
    private float moveSpeed = 5f;
    public PlayerAirChargeAttack_2EndState(PlayerManager _playerManager, string _animationName, bool _useRootMotionPart) : base(_playerManager, _animationName, _useRootMotionPart)
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

        //�����ɳ���Ϻ������̬��this -> idle
        AnimatorStateInfo stateInfo = playerManager.animator.GetCurrentAnimatorStateInfo(0);
        if(stateInfo.IsName(animationName) && playerManager.animator.IsInTransition(0))
        {
            ChangeState(playerManager.idleState);
        }
        else if(!stateInfo.IsName(animationName)) //�������������ʱ��ʩ���ƶ��ٶȣ������ָ�
        {
            playerManager.characterController.Move(playerManager.transform.forward * Time.deltaTime * moveSpeed);
        }
    }
}
