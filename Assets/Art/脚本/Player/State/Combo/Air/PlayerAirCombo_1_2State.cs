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

        //��ȡ��Ӧ����������
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

        //��ǰ���ڹ��ɣ��ҵ�ǰ���ŵĶ���Ϊcombo������Ŀ�겻Ϊcombo����Ϊ��������̬�������ص�airIdle
        // -> airIdle
        if (playerManager.animator.IsInTransition(0) && stateInfo.IsTag(PlayerAnimationName.Combo) && !nextStateInfo.IsTag(PlayerAnimationName.Combo))
        {
            ChangeState(playerManager.airIdleState);
            playerManager.animator.CrossFadeInFixedTime(playerManager.airIdleState.animationName, 0.25f);
        }
        //���������ﵽexitTime������û����ƶ����룬���˳�������̬
        // this -> airIdle
        else if (stateInfo.normalizedTime > comboConfi.exitTime)
        {
            if (playerManager.playerInputManager.moveRawAmount != 0)
            {
                //�л�Ϊ������̬idle
                ChangeState(playerManager.airIdleState);
                playerManager.animator.CrossFade(playerManager.airIdleState.animationName, 0.2f);
            }

        }
        //������̬ʱ������Ƿ񶯻��Ѿ��������
        else if (stateInfo.IsName(animationName))
        {
            //�ﵽ��Ԥ����ʱ�䣬����ڴ�ʱ̧�������������ڵ�����һ������ʱֱ���л���̬: this -> nextNormalCombo
            if (stateInfo.normalizedTime > comboConfi.preComboInputTime)
            {
                if (playerManager.playerInputManager.Mouse0UpInput())
                {
                    preInput = true;
                }
            }

            //nextComboTime�����������ʱ���ﵽָ��ʱ������chargeAttack_2;���̧��������������Ϊ��ͨ����
            if (stateInfo.normalizedTime > comboConfi.nextComboTime)
            {
                //�ȼ���Ƿ���Ԥ���룬�����Ԥ���룬��ֱ�ӽ�����һ������
                if (preInput)
                {
                    //�л���һ��������̬
                    ChangeState(playerManager.airCombo_1_3State);
                    // ����Ŀ����ת����
                    //SetRotateDir(playerManager.airCombo_1_3State);
                    playerManager.animator.CrossFade(playerManager.airCombo_1_3State.animationName, 0.05f);

                }
                //�ڴ�֮ǰ�ж������������룺�ﵽ�ɽ�����һ������ʱ����ж��Ƿ�ס���������������ס�ˣ��������������
                else if (playerManager.playerInputManager.Mouse0Input())
                {
                    chargeTimer += Time.deltaTime;
                    if (chargeTimer > chargeTime)
                    {
                        ChangeState(playerManager.airChargeAttack_2StartState);
                        playerManager.animator.CrossFade(playerManager.airChargeAttack_2StartState.animationName, 0.1f);
                    }
                }
                else if (playerManager.playerInputManager.Mouse0UpInput()) //̧����������Ϊ������ͨ����
                {
                    //�л���һ��������̬
                    ChangeState(playerManager.airCombo_1_3State);
                    playerManager.animator.CrossFade(playerManager.airCombo_1_3State.animationName, 0.05f);
                }
            }
        }
    }
}
