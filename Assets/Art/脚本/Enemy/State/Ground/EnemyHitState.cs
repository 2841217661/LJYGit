using System;
using UnityEngine;
using static EnemyAnimationName;
public class EnemyHitState : EnemyGroundState
{
    public EnumManager.EnemyDamageType damageType;
    public Vector3 hitDir;

    public EnemyHitState(EnemyManager _enemyManager, string _animationName, bool _useRootMotionPart) : base(_enemyManager, _animationName, _useRootMotionPart)
    {
    }

    public override void Enter()
    {
        base.Enter();

        enemyManager.rb.linearVelocity = Vector3.zero; //��ֹ����bug
    }

    public override void Exit()
    {
        base.Exit();

        //�˳�ʱ����ܻ�ö��
        damageType = EnumManager.EnemyDamageType.None;
        hitDir = Vector3.zero;

        //�ָ���ײ
        Physics.IgnoreLayerCollision(7, 8, false);
    }

    public override void Update()
    {
        base.Update();

        AnimatorStateInfo stateInfo = enemyManager.animator.GetCurrentAnimatorStateInfo(0);
        AnimatorStateInfo nextStateInfo = enemyManager.animator.GetNextAnimatorStateInfo(0);

        //this -> idle(����hit��̬����idle�ڼ�)
        if (stateInfo.IsTag(Hit) && nextStateInfo.IsName(enemyManager.idleState.animationName))
        {
            ChangeState(enemyManager.idleState);
        }
        else
        {
            SelectMoveAction(stateInfo.normalizedTime);
        }
    }

    private void SelectMoveAction(float _stateInfoNormalizedTime)
    {
        AnimatorStateInfo stateInfo = enemyManager.animator.GetCurrentAnimatorStateInfo(0);
        AnimatorStateInfo nextStateInfo = enemyManager.animator.GetNextAnimatorStateInfo(0);
        if (stateInfo.IsName(Get_Up) || nextStateInfo.IsName(Get_Up)) return;

        switch (damageType)
        {
            case EnumManager.EnemyDamageType.None:
                // ��λ��
                Debug.Log("�޷������ܻ�����λ��...");
                break;

            case EnumManager.EnemyDamageType.Light:
                HanleMovement(enemyManager.damageLightBackMoveCurve, _stateInfoNormalizedTime);
                break;

            case EnumManager.EnemyDamageType.Medium:
                HanleMovement(enemyManager.damageMediumBackMoveCurve, _stateInfoNormalizedTime);
                break;

            case EnumManager.EnemyDamageType.Heavy:
                HanleMovement(enemyManager.damageHeavyBackMoveCurve, _stateInfoNormalizedTime);
                break;
            default:
                Debug.LogError("����,�޸�����...");
                break;
        }
    }

    private void HanleMovement(AnimationCurve _curve, float _stateInfoNormalizedTime)
    {
        float moveAmount = _curve.Evaluate(_stateInfoNormalizedTime);

        Vector3 moveVector = hitDir * moveAmount * enemyManager.knockbackSpeedMul * Time.deltaTime;

        // ֱ���޸� transform
        enemyManager.transform.position += moveVector;
    }

}
