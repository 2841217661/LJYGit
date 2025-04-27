using NUnit.Framework;
using UnityEditor;
using UnityEngine;

public class PlayerComboState : PlayerState
{
    public Vector3 rotateDir; //��ǰ��ɫ�ĳ���������ĳ��������


    public PlayerComboState(PlayerManager _playerManager, string _animationName, bool _useRootMotionPart) : base(_playerManager, _animationName, _useRootMotionPart)
    {
    }

    public override void Enter()
    {
        base.Enter();

        rotateDir = Vector3.zero;
    }

    public override void Exit()
    {
        base.Exit();

    }

    public override void Update()
    {
        base.Update();

    }

    //�����ڵ�һ�ι���������ŵڶ��ι���������������뷽��ı䣬Ӧ����PlayerҲ���ж�Ӧ����ı�
    //������ڽ��й���ʱ��Χ�е��ˣ���player�ԽϿ���ٶ���ת�������
    protected virtual void HandleRotate()
    {
        if (rotateDir != Vector3.zero)
        {
            //������̬�н�����ת����
            // ����Ŀ����ת
            Quaternion newRotation = Quaternion.LookRotation(rotateDir);
            // ƽ����ת��Ŀ��
            Quaternion targetRotation = Quaternion.Slerp(playerManager.transform.rotation, newRotation, playerManager.comboRotateSpeed * Time.deltaTime);
            playerManager.transform.rotation = targetRotation;
        }
    }

    //����ҹ���ʱ�����Χ�е��ˣ������������
    protected virtual void SetRotateDir()
    {
        Vector3 nearEnemyPosition = playerManager.DetectionEnemyPositionInAttack();

        //������һ�����⣬���û�м�⵽���ˣ���᷵��zero��������������ָպ���zero��������жϻ�����(���ʼ���Ϊ0)
        if (nearEnemyPosition != Vector3.zero) 
        {
            //// ��������ĵ��ˣ�ֻ�� Y �ᣩ
            //Vector3 direction = nearEnemyPosition - playerManager.transform.position;
            //direction.y = 0f;

            //rotateDir = direction;

            Vector3 dir = nearEnemyPosition - playerManager.transform.position;
            dir.y = 0;
            playerManager.transform.rotation = Quaternion.LookRotation(dir);
            rotateDir = Vector3.zero;
        }
        else //û�е��ˣ����ݵ�ǰ�����������ķ����ж�
        {
            // ����Ŀ����ת����
            Vector3 targetRotationDirection = playerManager.playerCamera.transform.forward * playerManager.playerInputManager.movementInput.y
                                        + playerManager.playerCamera.transform.right * playerManager.playerInputManager.movementInput.x;

            targetRotationDirection.Normalize();
            targetRotationDirection.y = 0f;

            // û���������
            if (targetRotationDirection == Vector3.zero)
            {
                targetRotationDirection = playerManager.transform.forward;
            }

            rotateDir = targetRotationDirection;
        }
    }

    //������ʱ������������ˣ��谭player�ƶ�
    protected virtual void PreventMovement()
    {
        //������������ˣ���ֹrootMotion
        if (playerManager.AttackTouchEnemyDetecte())
        {
            useRootMotion = false;
        }
        else
        {
            useRootMotion = true;
        }
    }
}