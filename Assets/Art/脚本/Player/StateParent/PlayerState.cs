using UnityEngine;

public class PlayerState
{
    public PlayerManager playerManager;

    public string animationName;

    public bool useRootMotion;

    public PlayerState(PlayerManager _playerManager, string _animationName, bool _useRootMotionPart)
    {
        playerManager = _playerManager;
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

    public void ChangeState(PlayerState _newState)
    {
        playerManager.currentState.Exit(); // 转换姿态时调用当前姿态的退出方法
        playerManager.currentState = _newState; // 将当前的姿态转换为新的姿态
        playerManager.currentState.Enter(); // 调用新的姿态的进入方法
    }
}