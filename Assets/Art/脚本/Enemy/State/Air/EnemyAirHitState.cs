using System;
using UnityEngine;
using static EnemyAnimationName;
public class EnemyAirHitState : EnemyAirState
{
    public Action action_addFlyForce; //�����ܻ������ί��
    public Action action_addAirForce; //�����ܵ�������ί��

    private float flyForce = 450f;
    private float airForce = 3f;

    public EnumManager.EnemyDamageType damageType;
    public EnemyAirHitState(EnemyManager _enemyManager, string _animationName, bool _useRootMotionPart) : base(_enemyManager, _animationName, _useRootMotionPart)
    {
    }

    public override void Enter()
    {
        base.Enter();

    }

    public override void Exit()
    {
        base.Exit();

        damageType = EnumManager.EnemyDamageType.None;
    }

    public override void Update()
    {
        base.Update();

        //�䵽������
        if (enemyManager.IsGrounded() && enemyManager.rb.linearVelocity.y <= 0f && action_addFlyForce == null && action_addAirForce == null)
        {
            enemyManager.animator.CrossFade(Hit_Air_End, 0.1f);
            ChangeState(enemyManager.hitState);
        }
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();

        switch (damageType)
        {
            case EnumManager.EnemyDamageType.FlyAway:
                action_addFlyForce?.Invoke();
                break;
            case EnumManager.EnemyDamageType.AirLight:
                action_addAirForce?.Invoke();
                break;
            default:
                Debug.LogError("��������");
                break;
        }
    }

    //ί�з������ܵ��������͵Ĺ���ʱ��ʩ��һ�����ϵ���
    public void AddFlyForceOnce()
    {
        enemyManager.rb.linearVelocity = Vector3.zero;
        enemyManager.rb.AddForce(Vector3.up * flyForce, ForceMode.Impulse);
        // �Ƴ����������ȷ��ִֻ��һ��
        action_addFlyForce -= AddFlyForceOnce;
    }

    //ί�з������ڿ����ܵ�����ʱ��ʩ��һ�����ϵ���
    public void AddAirForceOnce()
    {
        enemyManager.rb.linearVelocity = new Vector3(0, airForce, 0f);

        // �Ƴ����������ȷ��ִֻ��һ��
        action_addAirForce -= AddAirForceOnce;

        if (enemyManager.player == null)
        {
            Debug.Log("playerΪ��");
            return;
        }

        // ����Ŀ��λ��
        Vector3 targetPos = enemyManager.player.position + enemyManager.player.forward * 1f;

        // ��һ����ײ���
        if (!Physics.Raycast(enemyManager.player.position, enemyManager.player.forward, out RaycastHit hit, 1f, LayerMask.GetMask("Default")))
        {
            // ���ǰ��û���ϰ�������ƶ�
            enemyManager.rb.MovePosition(targetPos);
        }
        else
        {
            // ��ǽ����ͣ��ԭ�ػ�������������
            Debug.Log("ǰ�����ϰ�����������");
        }
    }
}
