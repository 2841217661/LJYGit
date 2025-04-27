using UnityEngine;

public class PlayerAirChargeAttack_1StartState : PlayerAirComboState
{
    public bool isLeftDash;  //�Ƿ�Ϊ������
    private float dashTime = 0.5f; //�������ͷŵ�ʱ��
    private float dashTimer;
    private bool canDash; //�Ƿ���ͷ�
    public PlayerAirChargeAttack_1StartState(PlayerManager _playerManager, string _animationName, bool _useRootMotionPart) : base(_playerManager, _animationName, _useRootMotionPart)
    {
    }

    public override void Enter()
    {
        base.Enter();

        dashTimer = 0f;
        canDash = false;
    }

    public override void Exit()
    {
        base.Exit();

    }

    public override void Update()
    {
        base.Update();

        //����������Player������������ת
        HandleTheRotate();

        //����
        if (isLeftDash)
        {
            if (playerManager.playerInputManager.Mouse0Input()) //����
            {
                dashTimer += Time.deltaTime;
                if(dashTimer > dashTime) //�ɽ����ͷ�
                {
                    canDash = true;
                }
            }
            else if (playerManager.playerInputManager.Mouse0UpInput()) //�ͷ�
            {
                //�ж��Ƿ���Խ����ͷ�
                if (canDash) //�ͷţ�this -> loop
                {
                    ChangeState(playerManager.airChargeAttack_1LoopState);
                    playerManager.airChargeAttack_1LoopState.isLeftDash = true;
                }
                //���ɽ����ͷţ�this -> airIdle
                else
                {
                    ChangeState(playerManager.airIdleState);
                    playerManager.animator.CrossFade(playerManager.airIdleState.animationName, 0.2f);
                }
            }
        }
        //����
        else
        {
            if (playerManager.playerInputManager.Mouse1Input()) //����
            {
                dashTimer += Time.deltaTime;
                if (dashTimer > dashTime) //�ɽ����ͷ�
                {
                    canDash = true;
                }
            }
            else if (playerManager.playerInputManager.Mouse1UpInput()) //�ͷ�
            {
                //�ж��Ƿ���Խ����ͷ�
                if (canDash) //�ͷţ�this -> loop
                {
                    ChangeState(playerManager.airChargeAttack_1LoopState);
                    playerManager.airChargeAttack_1LoopState.isLeftDash = false;
                }
                //���ɽ����ͷţ�this -> airIdle
                else
                {
                    ChangeState(playerManager.airIdleState);
                    playerManager.animator.CrossFade(playerManager.airIdleState.animationName, 0.2f);
                }
            }
        }

    }

    private void HandleTheRotate()
    {
        //Ŀ�귽��
        Vector3 targetRotationDirection = playerManager.playerCamera.transform.forward;

        targetRotationDirection.Normalize();
        targetRotationDirection.y = 0; //ֻ��ˮƽ�������ת

        // ����Ŀ����ת
        Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);

        float rotationSpeed = 10f;

        // ƽ����ת��Ŀ��
        Quaternion targetRotation = Quaternion.Slerp(playerManager.transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
        playerManager.transform.rotation = targetRotation;
    }
}
