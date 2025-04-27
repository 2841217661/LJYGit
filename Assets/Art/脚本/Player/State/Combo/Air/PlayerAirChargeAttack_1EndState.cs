using UnityEngine;

public class PlayerAirChargeAttack_1EndState : PlayerAirComboState
{
    private float moveSpeed = 5f;

    public bool isLeftDash;
    public PlayerAirChargeAttack_1EndState(PlayerManager _playerManager, string _animationName, bool _useRootMotionPart) : base(_playerManager, _animationName, _useRootMotionPart)
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
        //�����ƶ��������ʱ��loop -> end�����룩������������Ҫʩ��һ���ƶ��������ָ�
        if (playerManager.animator.IsInTransition(0))
        {
            if (isLeftDash)
            {
                if (!stateInfo.IsName(PlayerAnimationName.AirChargeAttack_1_1End)) //����
                {
                    playerManager.characterController.Move(playerManager.transform.forward * Time.deltaTime * moveSpeed);

                }
                //��ǰ���ڶɳ�״̬�� //this -> idle
                else if (stateInfo.IsName(PlayerAnimationName.AirChargeAttack_1_1End)) //�ɳ�
                {
                    ChangeState(playerManager.idleState);
                    playerManager.animator.CrossFadeInFixedTime(playerManager.idleState.animationName, 0.1f);
                }
            }
            else
            {
                if (!stateInfo.IsName(PlayerAnimationName.AirChargeAttack_1_2End)) //����
                {
                    playerManager.characterController.Move(playerManager.transform.forward * Time.deltaTime * moveSpeed);

                }
                //��ǰ���ڶɳ�״̬�� //this -> idle
                else if (stateInfo.IsName(PlayerAnimationName.AirChargeAttack_1_2End)) //�ɳ�
                {
                    ChangeState(playerManager.idleState);
                    playerManager.animator.CrossFadeInFixedTime(playerManager.idleState.animationName, 0.1f);
                }
            }

        }
    }
}
