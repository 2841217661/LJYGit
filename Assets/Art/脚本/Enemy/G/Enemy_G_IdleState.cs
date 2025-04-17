using UnityEngine;

public class Enemy_G_IdleState : EnemyGuardState
{
    private float maxIdleTime = 3f; //最大滞留时间
    private float exitIdleTimer; //退出待机姿态计时器
    public Enemy_G_IdleState(EnemyManager _enemyManager, string _animationName, bool _useRootMotionPart) : base(_enemyManager, _animationName, _useRootMotionPart)
    {
    }

    public override void Enter()
    {
        base.Enter();

        exitIdleTimer = Random.Range(0,maxIdleTime); //进入巡逻姿态时随机减少滞留时间
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        exitIdleTimer += Time.deltaTime;

        //idle_N -> walk_N
        //退出待机姿态，进入巡逻
        if(exitIdleTimer > maxIdleTime)
        {
            ChangeState(enemyManager.G_walkState);
            enemyManager.animator.CrossFadeInFixedTime(enemyManager.G_walkState.animationName, 0.2f);

            //获取随机巡逻点
            enemyManager.G_walkState.targetGuardPoint = GetGuardPoint();
        }
    }

    private Transform GetGuardPoint()
    {
        int index = Random.Range(0,enemyManager.guardPointParent.childCount);

        return enemyManager.guardPointParent.GetChild(index).transform;
    }
}
