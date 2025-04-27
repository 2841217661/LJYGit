using UnityEngine;
using static EnemyAnimationName;

public class EnemyWalkState : EnemyGroundState
{
    public bool isFromMax; //从max这个范围切换过来的
    public bool isFromMid; //从mid这个范围切换过来的
    private float maxDrawedTime = 10f; //运行的最大被拉扯时间
    private float preventDrawedTimer; //防止被玩家拉扯计时器
    public EnemyWalkState(EnemyManager _enemyManager, string _animationName, bool _useRootMotionPart) : base(_enemyManager, _animationName, _useRootMotionPart)
    {
    }

    public override void Enter()
    {
        base.Enter();

        preventDrawedTimer = Random.Range(0, maxDrawedTime / 2f);//增加随机性

        enemyManager.agent.enabled = true;

        if (enemyManager.attackType == EnumManager.EnemyAttackType.Normal)
        {
            enemyManager.agent.SetDestination(SetGuardPoint());
        }

        enemyManager.agent.speed = enemyManager.walkSpeed;
        enemyManager.agent.angularSpeed = enemyManager.walkRotateSpeed;
    }

    public override void Exit()
    {
        base.Exit();

        enemyManager.agent.enabled = false;

        isFromMax = false;
        isFromMid = false;
    }

    public override void Update()
    {
        base.Update();

        //警戒状态下的逻辑
        if(enemyManager.attackType == EnumManager.EnemyAttackType.Normal)
        {
            //this -> chaseStart :检测玩家
            if (enemyManager.GuardForPlayer())
            {
                //如果检测到了玩家，进入追逐状态
                enemyManager.attackType = EnumManager.EnemyAttackType.Attack;

                //进入追逐状态
                ChangeState(enemyManager.chaseStartState);
            }
            //this -> idle: 到达目标巡逻点
            // 判断敌人是否已经到达目标点
            else if (!enemyManager.agent.pathPending &&  // 检查是否正在计算路径，false 表示路径已经计算完成
                enemyManager.agent.remainingDistance <= enemyManager.agent.stoppingDistance) // 剩余距离是否小于等于停止距离（默认是 0.5 米）
            {
                // 路径走完了，并且敌人已经不再移动
                if (!enemyManager.agent.hasPath || enemyManager.agent.velocity.sqrMagnitude == 0f)
                {
                    // !hasPath：没有有效路径，表示已经到达终点
                    // velocity.sqrMagnitude == 0：敌人当前速度为 0，即停止移动（sqrMagnitude 比 magnitude 性能更好）

                    ChangeState(enemyManager.idleState);
                    enemyManager.animator.CrossFadeInFixedTime(enemyManager.idleState.animationName, 0.2f);
                }
            }

        }
        //追逐状态下的逻辑
        else
        {
            //设置寻路目标点
            TrySetDestinationToPlayer();

            float possibility = Random.Range(0, 1f);
            //根据与玩家的距离进行行为决策
            switch (enemyManager.DetecteExploreType())
            {
                case EnumManager.EnemyExplorePlayerDistanceType.Max:
                    //TODO：可能是max切换过来的,添加防拉扯
                    if (isFromMax)
                    {
                        preventDrawedTimer += Time.deltaTime;
                        //已经被玩家拉扯得很久了,进行一些靠近玩家的行为
                        if (preventDrawedTimer > maxDrawedTime)
                        {
                            //this -> explore; this -> dashAttack; this -> combo_2(长距离挥击)
                            if (possibility < 0.3f)
                            {
                                //this -> dashAttack(长距离突击攻击): 30%进行突击
                                enemyManager.actionState.actionAnimationName = Dash_Attack;
                                ChangeState(enemyManager.actionState);
                            }
                            else
                            {
                                //this -> combo_2: 20%长距离攻击
                                //this -> dash_F: 30%向前闪避
                                //this -> roll_F: 20%向前翻滚
                                if (possibility < 0.5f)
                                {
                                    enemyManager.actionState.actionAnimationName = Combo_2;
                                }
                                else if (possibility < 0.8f)
                                {
                                    enemyManager.actionState.actionAnimationName = Dodge_F;
                                }
                                else
                                {
                                    enemyManager.actionState.actionAnimationName = Roll_F;
                                }
                                ChangeState(enemyManager.actionState);
                            }
                        }
                    }
                    break;
                case EnumManager.EnemyExplorePlayerDistanceType.Med:
                    preventDrawedTimer += Time.deltaTime;
                    //TODO：可能是mid切换过来的，也可能是从max到达mid这一瞬间切换过来的,添加防拉扯
                    //管他是mid切换过来的还是max->mid这一瞬间切换过来的，都是在这个姿态，preventDrawedTimer都会增加
                    //已经被拉扯得很久了，进行突击的行为
                    if (preventDrawedTimer > maxDrawedTime)
                    {
                        //this -> combo_1(中距离突击连击); this -> combo_3(中距离突击攻击);
                        if (possibility < 0.3f)
                        {
                            //this -> combo_1: 30%中距离突击连击
                            enemyManager.actionState.actionAnimationName = Combo_1;
                            ChangeState(enemyManager.actionState);
                        }
                        else if (possibility < 0.8f)
                        {
                            //this -> combo_3: 50%中距离突击攻击
                            enemyManager.actionState.actionAnimationName = Combo_3;
                            ChangeState(enemyManager.actionState);
                        }
                        else
                        {
                            //this -> roll_F: 20%向前翻滚
                            enemyManager.actionState.actionAnimationName = Roll_F;
                            ChangeState(enemyManager.actionState);
                        }
                    }
                    break;
                case EnumManager.EnemyExplorePlayerDistanceType.Min:
                    //当到达近距离时
                    //this -> explore; this -> combo_4(短距离攻击); this -> rollBack(向后翻滚); this -> combo_5(近距离连击)
                    if (possibility < 0.1f)
                    {
                        //this -> explore: 10%试探
                        ChangeState(enemyManager.exploreState);
                    }
                    else if (possibility < 0.5f)
                    {
                        //this -> combo_4: 40%短距离攻击
                        enemyManager.actionState.actionAnimationName = Combo_4;
                        ChangeState(enemyManager.actionState);
                    }
                    else if (possibility < 0.7f)
                    {
                        //this->combo_5: 20%短距离连击
                        enemyManager.actionState.actionAnimationName = Combo_5;
                        ChangeState(enemyManager.actionState);
                    }
                    else if (possibility < 0.9f)
                    {
                        //this -> rollBack: 20%向后翻滚
                        enemyManager.actionState.actionAnimationName = Roll_B;
                        ChangeState(enemyManager.actionState);
                    }
                    else
                    {
                        //this -> dash_B: 10%向后闪避
                        enemyManager.actionState.actionAnimationName = Dodge_B;
                        ChangeState(enemyManager.actionState);
                    }
                    break;
                default:
                    //this -> run: 玩家已经离开了可试探的范围，跑过去
                    ChangeState(enemyManager.runState);
                    enemyManager.animator.CrossFadeInFixedTime(enemyManager.runState.animationName, 0.2f);
                    break;
            }
        }
    }


    private Vector3 SetGuardPoint()
    {
        int index = Random.Range(0, enemyManager.guardPointParent.childCount);
        return enemyManager.guardPointParent.GetChild(index).transform.position;
    }
}
