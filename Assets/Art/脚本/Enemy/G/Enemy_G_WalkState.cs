using UnityEngine;

//正常状态下行走姿态(进行巡逻)
public class Enemy_G_WalkState : EnemyGuardState
{
    public Transform targetGuardPoint; //目标巡逻点


    public Enemy_G_WalkState(EnemyManager _enemyManager, string _animationName, bool _useRootMotionPart) : base(_enemyManager, _animationName, _useRootMotionPart)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        Move();
        Rotate();

        //到达目标巡逻点
        if(Vector3.Distance(enemyManager.transform.position, targetGuardPoint.position) < 0.1f)
        {
            ChangeState(enemyManager.G_idleState);
            enemyManager.animator.CrossFadeInFixedTime(enemyManager.G_idleState.animationName, 0.2f);
        }
    }

    private void Rotate()
    {
        // 计算目标方向
        Vector3 direction = (targetGuardPoint.position - enemyManager.transform.position).normalized;

        // 忽略y轴高度差（只在平面内旋转）
        direction.y = 0;

        if (direction == Vector3.zero) return;

        // 计算目标旋转
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // 平滑插值旋转（调整旋转速度可调）
        enemyManager.transform.rotation = Quaternion.Slerp(
            enemyManager.transform.rotation,
            targetRotation,
            Time.deltaTime * enemyManager.guardRotateSpeed
        );
    }

    private void Move()
    {
        // 获取当前方向
        Vector3 direction = (targetGuardPoint.position - enemyManager.transform.position).normalized;

        // 忽略y轴移动（保持在地面）
        direction.y = 0;

        // 移动
        enemyManager.transform.position += direction * enemyManager.guardWalkSpeed * Time.deltaTime;
    }

}
