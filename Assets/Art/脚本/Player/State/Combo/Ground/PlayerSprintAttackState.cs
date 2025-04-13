using UnityEngine;

public class PlayerSprintAttackState : PlayerGroundComboState
{
    //根据动画时长比决定动画退出时机
    private float attack_1Exit = 0.3f; 
    private float attack_2Exit = 0.5f; 

    public PlayerSprintAttackState(PlayerManager _playerManager, string _animationName, bool _useRootMotionPart) : base(_playerManager, _animationName, _useRootMotionPart)
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


        //获取动画状态机状态
        AnimatorStateInfo stateInfo = playerManager.animator.GetCurrentAnimatorStateInfo(0);

        //状态退出检查
        if (stateInfo.IsTag(animationName))
        {
            //sprintAttack_1 -> idle
            if (stateInfo.IsName(PlayerAnimationName.SprintAttack_1))
            {
                if (stateInfo.normalizedTime > attack_1Exit)
                {
                    //有移动输入，直接切换为基础姿态
                    if (playerManager.playerInputManager.moveRawAmount != 0)
                    {
                        ChangeState(playerManager.idleState);
                        playerManager.animator.CrossFadeInFixedTime(playerManager.idleState.animationName, 0.2f);
                    }
                    else
                    {
                        //动画已经播放完毕，正在进行过渡
                        if (playerManager.animator.IsInTransition(0))
                        {
                            ChangeState(playerManager.idleState);
                            playerManager.animator.CrossFadeInFixedTime(playerManager.idleState.animationName, 0.2f);
                        }
                    }
                }
            }
            //sprintAttack_2 -> idle
            else if (stateInfo.IsName(PlayerAnimationName.SprintAttack_2))
            {
                if (stateInfo.normalizedTime > attack_2Exit)
                {
                    //有移动输入，直接切换为基础姿态
                    if (playerManager.playerInputManager.moveRawAmount != 0)
                    {
                        ChangeState(playerManager.idleState);
                        playerManager.animator.CrossFadeInFixedTime(playerManager.idleState.animationName, 0.2f);
                    }
                    else
                    {
                        //动画已经播放完毕，正在进行过渡
                        if (playerManager.animator.IsInTransition(0))
                        {
                            ChangeState(playerManager.idleState);
                            playerManager.animator.CrossFadeInFixedTime(playerManager.idleState.animationName, 0.2f);
                        }
                    }
                }
            }
        }
    }
}
