using UnityEngine;

public class PlayerAirCombo_1_2State : PlayerAirComboState
{
    private PlayerComboConfi comboConfi;

    private float chargeTime = 0.2f;
    private float chargeTimer;

    private bool preInput;

    public PlayerAirCombo_1_2State(PlayerManager _playerManager, string _animationName, bool _useRootMotionPart) : base(_playerManager, _animationName, _useRootMotionPart)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //获取对应的连击配置
        comboConfi = playerManager.comboConfi.Find(c => c.comboName == animationName);

        chargeTimer = 0f;

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
        // -> airIdle
        if (playerManager.animator.IsInTransition(0) && stateInfo.IsTag(PlayerAnimationName.Combo) && !nextStateInfo.IsTag(PlayerAnimationName.Combo))
        {
            ChangeState(playerManager.airIdleState);
            playerManager.animator.CrossFadeInFixedTime(playerManager.airIdleState.animationName, 0.25f);
        }
        //攻击动画达到exitTime后，如果用户有移动输入，则退出连击姿态
        // this -> airIdle
        else if (stateInfo.normalizedTime > comboConfi.exitTime)
        {
            if (playerManager.playerInputManager.moveRawAmount != 0)
            {
                //切换为基础姿态idle
                ChangeState(playerManager.airIdleState);
                playerManager.animator.CrossFade(playerManager.airIdleState.animationName, 0.2f);
            }

        }
        //进入姿态时，检查是否动画已经过渡完毕
        else if (stateInfo.IsName(animationName))
        {
            //达到了预输入时间，如果在此时抬起了鼠标左键，在到达下一次连击时直接切换姿态: this -> nextNormalCombo
            if (stateInfo.normalizedTime > comboConfi.preComboInputTime)
            {
                if (playerManager.playerInputManager.Mouse0UpInput())
                {
                    preInput = true;
                }
            }

            //nextComboTime后进行蓄力计时，达到指定时间后进入chargeAttack_2;如果抬起了鼠标左键则视为普通攻击
            if (stateInfo.normalizedTime > comboConfi.nextComboTime)
            {
                //先检查是否有预输入，如果有预输入，则直接进入下一个连击
                if (preInput)
                {
                    //切换下一个连击姿态
                    ChangeState(playerManager.airCombo_1_3State);
                    // 计算目标旋转方向
                    //SetRotateDir(playerManager.airCombo_1_3State);
                    playerManager.animator.CrossFade(playerManager.airCombo_1_3State.animationName, 0.05f);

                }
                //在此之前判断蓄力攻击输入：达到可进行下一次连击时间后，判断是否按住了鼠标左键，如果按住了，则进行蓄力攻击
                else if (playerManager.playerInputManager.Mouse0Input())
                {
                    chargeTimer += Time.deltaTime;
                    if (chargeTimer > chargeTime)
                    {
                        ChangeState(playerManager.airChargeAttack_2StartState);
                        playerManager.animator.CrossFade(playerManager.airChargeAttack_2StartState.animationName, 0.1f);
                    }
                }
                else if (playerManager.playerInputManager.Mouse0UpInput()) //抬起鼠标左键视为进行普通连击
                {
                    //切换下一个连击姿态
                    ChangeState(playerManager.airCombo_1_3State);
                    playerManager.animator.CrossFade(playerManager.airCombo_1_3State.animationName, 0.05f);
                }
            }
        }
    }
}
