using System.Linq;
using UnityEngine;
using static EnemyAnimationName;

public class EnemyActionState : EnemyGroundState
{
    //��Щ�����ƶ��Ķ�����������ҷ�����ģ��ʹ��
    private string[] canBePrenventAction = 
        { Combo_1 , Combo_2 ,Combo_3,Combo_4,Combo_5,Dodge_B,Dodge_F,Dash_Attack};

    //���Դ�͸��ҵ���Ϊ
    private string[] canIgnoreColliderAction =
        {Dash_Attack};

    public string actionAnimationName;

    private bool hasRotated = false; // ���ֻ��תһ��

    public EnemyActionState(EnemyManager _enemyManager, string _animationName, bool _useRootMotionPart)
        : base(_enemyManager, _animationName, _useRootMotionPart)
    {
    }

    public override void Enter()
    {
        base.Enter();

        enemyManager.animator.applyRootMotion = true;

        hasRotated = false;

        // ���Ŷ���
        enemyManager.animator.CrossFadeInFixedTime(actionAnimationName, 0.2f);

        //������ײ���ⷽ������Щ�����ƶ��ٶ�̫�죬��Ҫ�߾�����ײ���
        SetColliderDetecteMethod();

        // ��� Y �ᶳ��
        enemyManager.rb.constraints |= RigidbodyConstraints.FreezePositionY;

        SetColliderIgnore();
    }

    public override void Exit()
    {
        base.Exit();

        enemyManager.animator.applyRootMotion = false;

        enemyManager.rb.collisionDetectionMode = CollisionDetectionMode.Discrete; //�ָ���ɢ���

        // �Ƴ� Y �ᶳ��
        enemyManager.rb.constraints &= ~RigidbodyConstraints.FreezePositionY;

        //�ָ���ײ
        Physics.IgnoreLayerCollision(7, 8, false);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (!hasRotated)
        {
            Vector3 faceDir = enemyManager.player.position - enemyManager.transform.position;
            faceDir.y = 0f;

            if (faceDir.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(faceDir);
                enemyManager.rb.MoveRotation(targetRotation);
                hasRotated = true;
            }
        }
    }

    public override void Update()
    {
        base.Update();

        AnimatorStateInfo stateInfo = enemyManager.animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName(enemyManager.idleState.animationName) && !enemyManager.animator.IsInTransition(0))
        {
            ChangeState(enemyManager.idleState);
        }
    }

    private void SetColliderDetecteMethod()
    {
        if (canBePrenventAction.Contains(actionAnimationName))
        {
            enemyManager.rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }
    }

    private void SetColliderIgnore()
    {
        if (canIgnoreColliderAction.Contains(actionAnimationName))
        {
            Physics.IgnoreLayerCollision(7,8,true);
        }
    }
}
