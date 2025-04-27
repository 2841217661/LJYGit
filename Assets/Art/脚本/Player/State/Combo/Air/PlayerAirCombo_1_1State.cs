using UnityEngine;

public class PlayerAirCombo_1_1State : PlayerAirComboState
{
    private PlayerComboConfi comboConfi;
    private bool preInput;
    public PlayerAirCombo_1_1State(PlayerManager _playerManager, string _animationName, bool _useRootMotionPart) : base(_playerManager, _animationName, _useRootMotionPart)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //获取对应的连击配置
        comboConfi = playerManager.comboConfi.Find(c => c.comboName == animationName);

        preInput = false;

        SetRotateDir();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        AnimatorStateInfo stateInfo = playerManager.animator.GetCurrentAnimatorStateInfo(0);
        AnimatorStateInfo nextStateInfo = playerManager.animator.GetNextAnimatorStateInfo(0);

        //当前正在过渡，且当前播放的动画为combo，过渡目标不为combo则视为该连击姿态结束，回到airIdle
        if(playerManager.animator.IsInTransition(0) && stateInfo.IsTag(PlayerAnimationName.Combo) && !nextStateInfo.IsTag(PlayerAnimationName.Combo))
        {
            ChangeState(playerManager.airIdleState);
            playerManager.animator.CrossFadeInFixedTime(playerManager.airIdleState.animationName, 0.25f);
        }

        //进入姿态时，检查是否动画已经过渡完毕
        if (stateInfo.IsName(animationName))
        {
            //检查是否到达可预输入时间
            if (!preInput && stateInfo.normalizedTime > comboConfi.preComboInputTime)
            {
                //预输入完后当动画达到nextComboTime时直接执行切换下一个连击姿态
                if (playerManager.playerInputManager.Mouse0DownInput())
                {
                    preInput = true;
                }
            }

            //未进行预输入，判断nextComboTime后是否进行了攻击输入
            if (stateInfo.normalizedTime > comboConfi.nextComboTime)
            {
                //到达下一次连击时间后，如果之前进行了预输入，则直接进入连击
                if (playerManager.playerInputManager.Mouse0DownInput() || preInput)
                {
                    //切换下一个连击姿态
                    ChangeState(playerManager.airCombo_1_2State);
                    playerManager.animator.CrossFade(playerManager.airCombo_1_2State.animationName, 0.05f);
                }
            }

            //攻击动画达到exitTime后，如果用户有移动输入，则退出连击姿态
            if (stateInfo.normalizedTime > comboConfi.exitTime)
            {
                if (playerManager.playerInputManager.moveRawAmount != 0)
                {
                    //切换为基础姿态idle
                    ChangeState(playerManager.airIdleState);
                    playerManager.animator.CrossFadeInFixedTime(playerManager.airIdleState.animationName, 0.2f);
                }

            }
        }
    }
}
