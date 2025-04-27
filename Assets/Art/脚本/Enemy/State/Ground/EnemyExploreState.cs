using UnityEngine;
using static EnemyAnimationName;
public class EnemyExploreState : EnemyGroundState
{
    private Vector3 localOffset;

    private float maxExploreTime = 4f;
    private float exitExploreTimer;
    public EnemyExploreState(EnemyManager _enemyManager, string _animationName, bool _useRootMotionPart) : base(_enemyManager, _animationName, _useRootMotionPart)
    {
    }

    public override void Enter()
    {
        base.Enter();

        Time.fixedDeltaTime = 0.01f;

        enemyManager.animator.CrossFadeInFixedTime(SetAnimationName(), 0.2f);

        exitExploreTimer = Random.Range(0, maxExploreTime / 2f);
    }

    public override void Exit()
    {
        base.Exit();

        Time.fixedDeltaTime = 0.02f;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        Locomoition();
    }

    public override void Update()
    {
        base.Update();

        exitExploreTimer += Time.deltaTime;
        if (exitExploreTimer < maxExploreTime) return;

        //this -> idle
        ChangeState(enemyManager.idleState);
        enemyManager.animator.CrossFadeInFixedTime(enemyManager.idleState.animationName, 0.2f);
    }


    private string SetAnimationName()
    {
        float possibility = Random.Range(0, 1f);
        switch (enemyManager.DetecteExploreType())
        {
            case EnumManager.EnemyExplorePlayerDistanceType.Max:
                if (possibility < 0.5f)
                {
                    localOffset = new Vector3(-1, 0, 0); // 左
                    return Explore_F_L_90;
                }
                else
                {
                    localOffset = new Vector3(1, 0, 0); // 右
                    return Explore_F_R_90;
                }

            case EnumManager.EnemyExplorePlayerDistanceType.Med:
                if (possibility < 0.5f)
                {
                    localOffset = new Vector3(-1, 0, 0); // 左
                    return Explore_F_L_90;
                }
                else if(possibility < 0.8f)
                {
                    localOffset = new Vector3(1, 0, 0); // 右
                    return Explore_F_R_90;
                }
                else
                {
                    localOffset = new Vector3(0, 0, -1);  // 后
                    return Explore_B;
                }
            case EnumManager.EnemyExplorePlayerDistanceType.Min:
                localOffset = new Vector3(0, 0, -1).normalized;
                return Explore_B; //后

            default:
                Debug.LogError("类型错误...");
                break;
        }

        return null;
    }

    private void Locomoition()
    {
        // 平滑面向玩家
        Vector3 faceDir = enemyManager.player.position - enemyManager.transform.position;
        faceDir.y = 0;
        faceDir = faceDir.normalized;
        Quaternion targetRotation = Quaternion.LookRotation(faceDir);
        enemyManager.transform.rotation = Quaternion.Slerp(enemyManager.transform.rotation, targetRotation, enemyManager.exploreRotateSpeed * Time.fixedDeltaTime);

        // 动态转换方向偏移为世界坐标
        Vector3 worldMoveDir = enemyManager.transform.TransformDirection(localOffset);

        // 设置刚体速度
        enemyManager.rb.linearVelocity = worldMoveDir * enemyManager.exploreMoveSpeed;
    }
}