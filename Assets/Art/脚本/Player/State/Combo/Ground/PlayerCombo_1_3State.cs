using UnityEngine;

public class PlayerCombo_1_3State : PlayerGroundComboState
{
    private PlayerComboConfi comboConfi;

    private float chargeTime = 0.15f;
    private float chargeTimer;

    private bool preInput;

    public PlayerCombo_1_3State(PlayerManager _playerManager, string _animationName, bool _useRootMotionPart) : base(_playerManager, _animationName, _useRootMotionPart)
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

        HandleRotate();

        PreventMovement();

        AnimatorStateInfo stateInfo = playerManager.animator.GetCurrentAnimatorStateInfo(0);
        AnimatorStateInfo nextStateInfo = playerManager.animator.GetNextAnimatorStateInfo(0);

        //��ǰ���ڹ��ɣ��ҵ�ǰ���ŵĶ���Ϊcombo������Ŀ�겻Ϊcombo����Ϊ��������̬�������ص�idle
        // -> idle
        if (playerManager.animator.IsInTransition(0) && stateInfo.IsTag(PlayerAnimationName.Combo) && !nextStateInfo.IsTag(PlayerAnimationName.Combo))
        {
            ChangeState(playerManager.idleState);
        }

        //���������ﵽexitTime������û����ƶ����룬���˳�������̬
        if (stateInfo.normalizedTime > comboConfi.exitTime)
        {
            if (playerManager.playerInputManager.moveRawAmount != 0)
            {
                //�л�Ϊ������̬idle
                ChangeState(playerManager.idleState);
                playerManager.animator.CrossFadeInFixedTime(playerManager.idleState.animationName, 0.2f);
            }

        }

        //������̬ʱ������Ƿ񶯻��Ѿ��������
        if (stateInfo.IsName(animationName))
        {


            //�ﵽ��Ԥ����ʱ�䣬����ڴ�ʱ̧�������������ڵ�����һ������ʱֱ���л���̬: this -> nextNormalCombo
            if (stateInfo.normalizedTime > comboConfi.preComboInputTime)
            {
                if (playerManager.playerInputManager.Mouse0UpInput())
                {
                    preInput = true;
                }
            }

            //nextComboTime�����������ʱ���ﵽָ��ʱ������chargeAttack_1;���̧��������������Ϊ��ͨ����
            if (stateInfo.normalizedTime > comboConfi.nextComboTime)
            {
                //�Ѿ�����Ԥ���룺�л�Ϊ��һ��������̬
                if (preInput)
                {
                    //�л���һ��������̬
                    ChangeState(playerManager.combo_1_4State);
                    playerManager.animator.CrossFade(playerManager.combo_1_4State.animationName, 0.05f);
                }


                //������һ������ʱ������֮ǰ������Ԥ���룬��ֱ�ӽ�������
                //�ڴ�֮ǰ�ж������������룺�ﵽ�ɽ�����һ������ʱ����ж��Ƿ�ס���������������ס�ˣ��������������
                if (playerManager.playerInputManager.Mouse0Input())
                {
                    chargeTimer += Time.deltaTime;
                    if (chargeTimer > chargeTime)
                    {
                        ChangeState(playerManager.chargeAttack_1State);
                        playerManager.animator.CrossFade(playerManager.chargeAttack_1State.animationName, 0.2f);
                    }
                }
                else if(playerManager.playerInputManager.Mouse0UpInput()) //̧����������Ϊ������ͨ����
                {
                    //�л���һ��������̬
                    ChangeState(playerManager.combo_1_4State);
                    playerManager.animator.CrossFade(playerManager.combo_1_4State.animationName, 0.05f);
                }
            }


        }
    }
}
