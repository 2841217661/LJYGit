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

        //��ȡ��Ӧ����������
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

        //��ǰ���ڹ��ɣ��ҵ�ǰ���ŵĶ���Ϊcombo������Ŀ�겻Ϊcombo����Ϊ��������̬�������ص�airIdle
        if(playerManager.animator.IsInTransition(0) && stateInfo.IsTag(PlayerAnimationName.Combo) && !nextStateInfo.IsTag(PlayerAnimationName.Combo))
        {
            ChangeState(playerManager.airIdleState);
            playerManager.animator.CrossFadeInFixedTime(playerManager.airIdleState.animationName, 0.25f);
        }

        //������̬ʱ������Ƿ񶯻��Ѿ��������
        if (stateInfo.IsName(animationName))
        {
            //����Ƿ񵽴��Ԥ����ʱ��
            if (!preInput && stateInfo.normalizedTime > comboConfi.preComboInputTime)
            {
                //Ԥ������󵱶����ﵽnextComboTimeʱֱ��ִ���л���һ��������̬
                if (playerManager.playerInputManager.Mouse0DownInput())
                {
                    preInput = true;
                }
            }

            //δ����Ԥ���룬�ж�nextComboTime���Ƿ�����˹�������
            if (stateInfo.normalizedTime > comboConfi.nextComboTime)
            {
                //������һ������ʱ������֮ǰ������Ԥ���룬��ֱ�ӽ�������
                if (playerManager.playerInputManager.Mouse0DownInput() || preInput)
                {
                    //�л���һ��������̬
                    ChangeState(playerManager.airCombo_1_2State);
                    playerManager.animator.CrossFade(playerManager.airCombo_1_2State.animationName, 0.05f);
                }
            }

            //���������ﵽexitTime������û����ƶ����룬���˳�������̬
            if (stateInfo.normalizedTime > comboConfi.exitTime)
            {
                if (playerManager.playerInputManager.moveRawAmount != 0)
                {
                    //�л�Ϊ������̬idle
                    ChangeState(playerManager.airIdleState);
                    playerManager.animator.CrossFadeInFixedTime(playerManager.airIdleState.animationName, 0.2f);
                }

            }
        }
    }
}
