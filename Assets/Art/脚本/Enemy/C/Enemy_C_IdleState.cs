using UnityEngine;

public class Enemy_C_IdleState : EnemyChaseState
{
    //添加滞留功能，适当降低敌人反应能力
    private float maxIdleTime = 1f; //最大滞留时间
    private float exitIdleTimer; //退出待机姿态计时器
    public Enemy_C_IdleState(EnemyManager _enemyManager, string _animationName, bool _useRootMotionPart) : base(_enemyManager, _animationName, _useRootMotionPart)
    {
    }

    public override void Enter()
    {
        base.Enter();

        exitIdleTimer = Random.Range(0, maxIdleTime); //每次随机减少滞留时间
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        exitIdleTimer += Time.deltaTime;

        //滞留时间未到，继续待机
        if (exitIdleTimer < maxIdleTime)
            return;

        //根据玩家一系列信息进行行为决策


    }
}
