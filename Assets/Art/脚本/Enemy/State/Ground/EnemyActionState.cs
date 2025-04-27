using System.Linq;
using UnityEngine;
using static EnemyAnimationName;

public class EnemyActionState : EnemyGroundState
{
    //有些快速移动的动作可以与玩家发生穿模，使用
    private string[] canBePrenventAction = 
        { Combo_1 , Combo_2 ,Combo_3,Combo_4,Combo_5,Dodge_B,Dodge_F,Dash_Attack};

    //可以穿透玩家的行为
    private string[] canIgnoreColliderAction =
        {Dash_Attack};

    public string actionAnimationName;

    private bool hasRotated = false; // 标记只旋转一次

    public EnemyActionState(EnemyManager _enemyManager, string _animationName, bool _useRootMotionPart)
        : base(_enemyManager, _animationName, _useRootMotionPart)
    {
    }

    public override void Enter()
    {
        base.Enter();

        enemyManager.animator.applyRootMotion = true;

        hasRotated = false;

        // 播放动画
        enemyManager.animator.CrossFadeInFixedTime(actionAnimationName, 0.2f);

        //设置碰撞体检测方法，有些动画移动速度太快，需要高精度碰撞检测
        SetColliderDetecteMethod();

        // 添加 Y 轴冻结
        enemyManager.rb.constraints |= RigidbodyConstraints.FreezePositionY;

        SetColliderIgnore();
    }

    public override void Exit()
    {
        base.Exit();

        enemyManager.animator.applyRootMotion = false;

        enemyManager.rb.collisionDetectionMode = CollisionDetectionMode.Discrete; //恢复离散检测

        // 移除 Y 轴冻结
        enemyManager.rb.constraints &= ~RigidbodyConstraints.FreezePositionY;

        //恢复碰撞
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
