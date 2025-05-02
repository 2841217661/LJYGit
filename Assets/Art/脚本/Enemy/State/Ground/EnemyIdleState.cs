using UnityEngine;
using static EnemyAnimationName;
public class EnemyIdleState : EnemyGroundState
{
    private float maxIdleTime = 0.2f; //每次行为结束时需要待机的最大时间
    private float exitIdleTimer; //待机计时器
    public EnemyIdleState(EnemyManager _enemyManager, string _animationName, bool _useRootMotionPart) : base(_enemyManager, _animationName, _useRootMotionPart)
    {
    }

    public override void Enter()
    {
        base.Enter();

        enemyManager.agent.enabled = false;

        //每次进入该姿态时随机减少待机时间
        exitIdleTimer = Random.Range(0,maxIdleTime);
    }

    public override void Exit()
    {
        base.Exit();

    }

    public override void Update()
    {
        base.Update();

        exitIdleTimer += Time.deltaTime;

        if (exitIdleTimer < maxIdleTime) return; //未到达退出待机时间，等待下一帧

        AnimatorStateInfo stateInfo = enemyManager.animator.GetCurrentAnimatorStateInfo(0);
        if(stateInfo.IsName(animationName) && !enemyManager.animator.IsInTransition(0))
        {

        }
        else
        {
            return;
        }

        //警戒状态下的行为
        if(enemyManager.attackType == EnumManager.EnemyAttackType.Normal)
        {
            //this -> chaseStart:检测玩家
            if (enemyManager.GuardForPlayer())
            {
                //如果检测到了玩家，进入追逐状态
                enemyManager.attackType = EnumManager.EnemyAttackType.Attack;
                //进入追逐状态
                ChangeState(enemyManager.chaseStartState);
            }
            //this -> walk:巡逻
            else
            {
                enemyManager.animator.CrossFadeInFixedTime(enemyManager.walkState.animationName, 0.2f);
                ChangeState(enemyManager.walkState);
            }
        }
        //追逐状态下的行为
        else
        {
            float possibility = Random.Range(0, 1f);
            switch (enemyManager.DetecteExploreType())
            {
                case EnumManager.EnemyExplorePlayerDistanceType.Max:
                    //this -> explore; this -> dashAttack; this -> walk; this -> combo_2(长距离挥击)
                    if (possibility < 0.2f)
                    {
                        //this -> dashAttack(长距离突击攻击): 20%进行突击
                        enemyManager.actionState.actionAnimationName = Dash_Attack;
                        ChangeState(enemyManager.actionState);
                    }
                    else if (possibility < 0.4f)
                    {
                        //this -> explore: 20%继续试探
                        ChangeState(enemyManager.exploreState);
                    }
                    else if (possibility < 0.7f)
                    {
                        //this -> walk: 30%靠近玩家
                        enemyManager.walkState.isFromMax = true;
                        enemyManager.animator.CrossFadeInFixedTime(enemyManager.walkState.animationName, 0.2f);
                        ChangeState(enemyManager.walkState);
                    }
                    else if(possibility < 0.8f)
                    {
                        //this -> combo_1: 10%中距离突击连击
                        enemyManager.actionState.actionAnimationName = Combo_1;
                        ChangeState(enemyManager.actionState);
                    }
                    else
                    {
                        //this -> combo_2: 10%长距离攻击
                        //this -> dash_F: 5%向前闪避
                        //this -> roll_F: 5%向前翻滚
                        if(possibility < 0.9f)
                        {
                            enemyManager.actionState.actionAnimationName = Combo_2;
                        }
                        else if(possibility < 0.95f)
                        {
                            enemyManager.actionState.actionAnimationName = Dodge_F;
                        }
                        else
                        {
                            enemyManager.actionState.actionAnimationName = Roll_F;
                        }
                        ChangeState(enemyManager.actionState);
                    }
                    break;
                case EnumManager.EnemyExplorePlayerDistanceType.Med:
                    //this -> explore; this -> walk; this -> combo_1(中距离突击连击); this -> combo_3(中距离突击攻击);
                    if (possibility < 0.2f)
                    {
                        //this -> explore: 20%继续试探
                        ChangeState(enemyManager.exploreState);
                    }
                    else if (possibility < 0.5f)
                    {
                        //this -> walk: 30%靠近玩家
                        enemyManager.walkState.isFromMax = true;
                        enemyManager.animator.CrossFadeInFixedTime(enemyManager.walkState.animationName, 0.2f);
                        ChangeState(enemyManager.walkState);
                    }
                    else if (possibility < 0.7f)
                    {
                        //this -> combo_1: 20%中距离突击连击
                        enemyManager.actionState.actionAnimationName = Combo_1;
                        ChangeState(enemyManager.actionState);
                    }
                    else if(possibility < 0.9f)
                    {
                        //this -> combo_3: 20%中距离突击攻击
                        enemyManager.actionState.actionAnimationName = Combo_3;
                        ChangeState(enemyManager.actionState);
                    }
                    else
                    {
                        //this -> roll_F: 10%向前翻滚
                        enemyManager.actionState.actionAnimationName = Roll_F;
                        ChangeState(enemyManager.actionState);
                    }
                    break;
                case EnumManager.EnemyExplorePlayerDistanceType.Min:
                    //this -> explore; this -> combo_4(短距离攻击); this -> rollBack(向后翻滚); this -> combo_5(近距离连击)
                    if (possibility < 0.2f)
                    {
                        //this -> explore: 20%继续试探
                        ChangeState(enemyManager.exploreState);
                    }
                    else if (possibility < 0.35f)
                    {
                        //this -> combo_4: 10%短距离攻击
                        enemyManager.actionState.actionAnimationName = Combo_4;
                        ChangeState(enemyManager.actionState);
                    }
                    else if (possibility < 0.55f)
                    {
                        //this -> combo_3: 20%中距离突击攻击
                        enemyManager.actionState.actionAnimationName = Combo_3;
                        ChangeState(enemyManager.actionState);
                    }
                    else if(possibility < 0.7f)
                    {
                        //this->combo_5: 15%短距离连击
                        enemyManager.actionState.actionAnimationName = Combo_5;
                        ChangeState(enemyManager.actionState);
                    }
                    else if(possibility < 0.9f)
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
                    //this -> run: 距离玩家较远，跑过去
                    enemyManager.animator.CrossFadeInFixedTime(enemyManager.runState.animationName, 0.2f);
                    ChangeState(enemyManager.runState);
                    break;
            }

        }
    }
}
