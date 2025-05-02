using UnityEngine;
using static EnemyAnimationName;
public class EnemyIdleState : EnemyGroundState
{
    private float maxIdleTime = 0.2f; //ÿ����Ϊ����ʱ��Ҫ���������ʱ��
    private float exitIdleTimer; //������ʱ��
    public EnemyIdleState(EnemyManager _enemyManager, string _animationName, bool _useRootMotionPart) : base(_enemyManager, _animationName, _useRootMotionPart)
    {
    }

    public override void Enter()
    {
        base.Enter();

        enemyManager.agent.enabled = false;

        //ÿ�ν������̬ʱ������ٴ���ʱ��
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

        if (exitIdleTimer < maxIdleTime) return; //δ�����˳�����ʱ�䣬�ȴ���һ֡

        AnimatorStateInfo stateInfo = enemyManager.animator.GetCurrentAnimatorStateInfo(0);
        if(stateInfo.IsName(animationName) && !enemyManager.animator.IsInTransition(0))
        {

        }
        else
        {
            return;
        }

        //����״̬�µ���Ϊ
        if(enemyManager.attackType == EnumManager.EnemyAttackType.Normal)
        {
            //this -> chaseStart:������
            if (enemyManager.GuardForPlayer())
            {
                //�����⵽����ң�����׷��״̬
                enemyManager.attackType = EnumManager.EnemyAttackType.Attack;
                //����׷��״̬
                ChangeState(enemyManager.chaseStartState);
            }
            //this -> walk:Ѳ��
            else
            {
                enemyManager.animator.CrossFadeInFixedTime(enemyManager.walkState.animationName, 0.2f);
                ChangeState(enemyManager.walkState);
            }
        }
        //׷��״̬�µ���Ϊ
        else
        {
            float possibility = Random.Range(0, 1f);
            switch (enemyManager.DetecteExploreType())
            {
                case EnumManager.EnemyExplorePlayerDistanceType.Max:
                    //this -> explore; this -> dashAttack; this -> walk; this -> combo_2(������ӻ�)
                    if (possibility < 0.2f)
                    {
                        //this -> dashAttack(������ͻ������): 20%����ͻ��
                        enemyManager.actionState.actionAnimationName = Dash_Attack;
                        ChangeState(enemyManager.actionState);
                    }
                    else if (possibility < 0.4f)
                    {
                        //this -> explore: 20%������̽
                        ChangeState(enemyManager.exploreState);
                    }
                    else if (possibility < 0.7f)
                    {
                        //this -> walk: 30%�������
                        enemyManager.walkState.isFromMax = true;
                        enemyManager.animator.CrossFadeInFixedTime(enemyManager.walkState.animationName, 0.2f);
                        ChangeState(enemyManager.walkState);
                    }
                    else if(possibility < 0.8f)
                    {
                        //this -> combo_1: 10%�о���ͻ������
                        enemyManager.actionState.actionAnimationName = Combo_1;
                        ChangeState(enemyManager.actionState);
                    }
                    else
                    {
                        //this -> combo_2: 10%�����빥��
                        //this -> dash_F: 5%��ǰ����
                        //this -> roll_F: 5%��ǰ����
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
                    //this -> explore; this -> walk; this -> combo_1(�о���ͻ������); this -> combo_3(�о���ͻ������);
                    if (possibility < 0.2f)
                    {
                        //this -> explore: 20%������̽
                        ChangeState(enemyManager.exploreState);
                    }
                    else if (possibility < 0.5f)
                    {
                        //this -> walk: 30%�������
                        enemyManager.walkState.isFromMax = true;
                        enemyManager.animator.CrossFadeInFixedTime(enemyManager.walkState.animationName, 0.2f);
                        ChangeState(enemyManager.walkState);
                    }
                    else if (possibility < 0.7f)
                    {
                        //this -> combo_1: 20%�о���ͻ������
                        enemyManager.actionState.actionAnimationName = Combo_1;
                        ChangeState(enemyManager.actionState);
                    }
                    else if(possibility < 0.9f)
                    {
                        //this -> combo_3: 20%�о���ͻ������
                        enemyManager.actionState.actionAnimationName = Combo_3;
                        ChangeState(enemyManager.actionState);
                    }
                    else
                    {
                        //this -> roll_F: 10%��ǰ����
                        enemyManager.actionState.actionAnimationName = Roll_F;
                        ChangeState(enemyManager.actionState);
                    }
                    break;
                case EnumManager.EnemyExplorePlayerDistanceType.Min:
                    //this -> explore; this -> combo_4(�̾��빥��); this -> rollBack(��󷭹�); this -> combo_5(����������)
                    if (possibility < 0.2f)
                    {
                        //this -> explore: 20%������̽
                        ChangeState(enemyManager.exploreState);
                    }
                    else if (possibility < 0.35f)
                    {
                        //this -> combo_4: 10%�̾��빥��
                        enemyManager.actionState.actionAnimationName = Combo_4;
                        ChangeState(enemyManager.actionState);
                    }
                    else if (possibility < 0.55f)
                    {
                        //this -> combo_3: 20%�о���ͻ������
                        enemyManager.actionState.actionAnimationName = Combo_3;
                        ChangeState(enemyManager.actionState);
                    }
                    else if(possibility < 0.7f)
                    {
                        //this->combo_5: 15%�̾�������
                        enemyManager.actionState.actionAnimationName = Combo_5;
                        ChangeState(enemyManager.actionState);
                    }
                    else if(possibility < 0.9f)
                    {
                        //this -> rollBack: 20%��󷭹�
                        enemyManager.actionState.actionAnimationName = Roll_B;
                        ChangeState(enemyManager.actionState);
                    }
                    else
                    {
                        //this -> dash_B: 10%�������
                        enemyManager.actionState.actionAnimationName = Dodge_B;
                        ChangeState(enemyManager.actionState);
                    }
                    break;
                default:
                    //this -> run: ������ҽ�Զ���ܹ�ȥ
                    enemyManager.animator.CrossFadeInFixedTime(enemyManager.runState.animationName, 0.2f);
                    ChangeState(enemyManager.runState);
                    break;
            }

        }
    }
}
