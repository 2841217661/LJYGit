using UnityEngine;

public class EnemyState
{
    public EnemyManager enemyManager;

    public string animationName;

    public bool useRootMotion;

    public EnemyState(EnemyManager _enemyManager, string _animationName, bool _useRootMotionPart)
    {
        enemyManager = _enemyManager;
        animationName = _animationName;
        useRootMotion = _useRootMotionPart;
    }

    public virtual void Enter()
    {
        //进入逻辑
    }

    public virtual void Update()
    {
        // 更新逻辑
    }

    public virtual void Exit()
    {
        // 退出逻辑
    }

    public virtual void FixedUpdate()
    {
        // 物理更新逻辑
    }


    public void ChangeState(EnemyState _newState)
    {
        enemyManager.currentState.Exit(); // 转换姿态时调用当前姿态的退出方法
        enemyManager.currentState = _newState; // 将当前的姿态转换为新的姿态
        enemyManager.currentState.Enter(); // 调用新的姿态的进入方法
    }
}
