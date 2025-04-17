using UnityEngine;

public class Enemy_C_PrepareState : EnemyChaseState
{
    public Enemy_C_PrepareState(EnemyManager _enemyManager, string _animationName, bool _useRootMotionPart) : base(_enemyManager, _animationName, _useRootMotionPart)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //面向player
        SetFaceDir();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        //this -> c_idle: 动画播放完毕
        AnimatorStateInfo stateInfo = enemyManager.animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName(animationName) && enemyManager.animator.IsInTransition(0))
        {
            ChangeState(enemyManager.C_idleState);
        }

        //this -> c_hit: 动画播放阶段被攻击打断，直接激活武器并进入c_hit姿态

    }

    private void SetFaceDir()
    {
        Vector3 dir = enemyManager.guardTargetManager.transform.position - enemyManager.transform.position;
        dir.y = 0;
        dir = dir.normalized;
        float angle = Vector3.SignedAngle(new Vector3(enemyManager.transform.forward.x, 0f, enemyManager.transform.forward.z).normalized, dir,Vector3.up);
        //根据前方向量与相对于玩家的向量决定转向动画
        string name = angle switch
        {
            >= 120f => "Turn_180_R",
            < -120 => "Turn_180_L",
            >= 60 and < 120 => "Turn_90_R",
            < -60 and > -120 => "Turn_90_L",
            _ => ""
        };

        if(name != "")
        {
            //融合下半身旋转动画
            enemyManager.animator.CrossFadeInFixedTime(name, 0.2f, 1);
        }

        enemyManager.animator.CrossFadeInFixedTime(animationName, 0.2f);
    }
}
