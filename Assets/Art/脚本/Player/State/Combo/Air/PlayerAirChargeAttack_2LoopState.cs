using UnityEngine;

public class PlayerAirChargeAttack_2LoopState : PlayerAirComboState
{
    private float moveSpeed = 15f;
    private float durationTime = 1f;
    private float durationTimer;
    public PlayerAirChargeAttack_2LoopState(PlayerManager _playerManager, string _animationName, bool _useRootMotionPart) : base(_playerManager, _animationName, _useRootMotionPart)
    {
    }

    public override void Enter()
    {
        base.Enter();

        durationTimer = 0f;

        Physics.IgnoreLayerCollision(7, 8, true);
    }

    public override void Exit()
    {
        base.Exit();

        Physics.IgnoreLayerCollision(7, 8, false);
    }

    public override void Update()
    {
        base.Update();

        durationTimer += Time.deltaTime;

        //����ʱ�������this -> end
        if (durationTimer > durationTime)
        {
            ChangeState(playerManager.airChargeAttack_2EndState);
            playerManager.animator.CrossFadeInFixedTime(playerManager.airChargeAttack_2EndState.animationName, 0.2f);
        }
        else //��������...
        {
            //Ӧ������
            playerManager.HandleGravity();

            //�����ƶ�
            playerManager.characterController.Move(playerManager.transform.forward * moveSpeed * Time.deltaTime);
        }
    }
}
