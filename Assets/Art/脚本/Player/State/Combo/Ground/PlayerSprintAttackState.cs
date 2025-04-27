using System;
using UnityEngine;

public class PlayerSprintAttackState : PlayerGroundComboState
{
    //根据动画时长比决定动画退出时机
    private float attack_1Exit = 0.3f; 
    private float attack_2Exit = 0.5f;

    private Action action_sprintAttackEnemy; //在冲刺攻击过程只会对敌人造成一次伤害

    public PlayerSprintAttackState(PlayerManager _playerManager, string _animationName, bool _useRootMotionPart) : base(_playerManager, _animationName, _useRootMotionPart)
    {
    }

    public override void Enter()
    {
        base.Enter();

        action_sprintAttackEnemy += AttackEnemy;
    }

    public override void Exit()
    {
        base.Exit();

        action_sprintAttackEnemy -= AttackEnemy;
    }

    public override void Update()
    {
        base.Update();

        action_sprintAttackEnemy?.Invoke();


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
                        }
                    }
                }
            }
        }
    }

    //冲刺过程攻击敌人
    private void AttackEnemy()
    {
        if (playerManager.AttackDetecte(3))
        {
            //如果玩家和敌人同时进行突击攻击，玩家在攻击到敌人的瞬间，敌人进入hit姿态，此时两者碰撞体会有一部分重合，
            //由于玩家碰撞体是固态的，就会导致敌人在切换到hit时被弹飞

            //忽略碰撞，敌人在hit姿态结束时会恢复碰撞
            Physics.IgnoreLayerCollision(7, 8, true);

            action_sprintAttackEnemy -= AttackEnemy;
        }
    }
}
