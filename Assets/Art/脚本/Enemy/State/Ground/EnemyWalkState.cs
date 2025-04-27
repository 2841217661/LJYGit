using UnityEngine;
using static EnemyAnimationName;

public class EnemyWalkState : EnemyGroundState
{
    public bool isFromMax; //��max�����Χ�л�������
    public bool isFromMid; //��mid�����Χ�л�������
    private float maxDrawedTime = 10f; //���е��������ʱ��
    private float preventDrawedTimer; //��ֹ�����������ʱ��
    public EnemyWalkState(EnemyManager _enemyManager, string _animationName, bool _useRootMotionPart) : base(_enemyManager, _animationName, _useRootMotionPart)
    {
    }

    public override void Enter()
    {
        base.Enter();

        preventDrawedTimer = Random.Range(0, maxDrawedTime / 2f);//���������

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

        //����״̬�µ��߼�
        if(enemyManager.attackType == EnumManager.EnemyAttackType.Normal)
        {
            //this -> chaseStart :������
            if (enemyManager.GuardForPlayer())
            {
                //�����⵽����ң�����׷��״̬
                enemyManager.attackType = EnumManager.EnemyAttackType.Attack;

                //����׷��״̬
                ChangeState(enemyManager.chaseStartState);
            }
            //this -> idle: ����Ŀ��Ѳ�ߵ�
            // �жϵ����Ƿ��Ѿ�����Ŀ���
            else if (!enemyManager.agent.pathPending &&  // ����Ƿ����ڼ���·����false ��ʾ·���Ѿ��������
                enemyManager.agent.remainingDistance <= enemyManager.agent.stoppingDistance) // ʣ������Ƿ�С�ڵ���ֹͣ���루Ĭ���� 0.5 �ף�
            {
                // ·�������ˣ����ҵ����Ѿ������ƶ�
                if (!enemyManager.agent.hasPath || enemyManager.agent.velocity.sqrMagnitude == 0f)
                {
                    // !hasPath��û����Ч·������ʾ�Ѿ������յ�
                    // velocity.sqrMagnitude == 0�����˵�ǰ�ٶ�Ϊ 0����ֹͣ�ƶ���sqrMagnitude �� magnitude ���ܸ��ã�

                    ChangeState(enemyManager.idleState);
                    enemyManager.animator.CrossFadeInFixedTime(enemyManager.idleState.animationName, 0.2f);
                }
            }

        }
        //׷��״̬�µ��߼�
        else
        {
            //����Ѱ·Ŀ���
            TrySetDestinationToPlayer();

            float possibility = Random.Range(0, 1f);
            //��������ҵľ��������Ϊ����
            switch (enemyManager.DetecteExploreType())
            {
                case EnumManager.EnemyExplorePlayerDistanceType.Max:
                    //TODO��������max�л�������,��ӷ�����
                    if (isFromMax)
                    {
                        preventDrawedTimer += Time.deltaTime;
                        //�Ѿ�����������úܾ���,����һЩ������ҵ���Ϊ
                        if (preventDrawedTimer > maxDrawedTime)
                        {
                            //this -> explore; this -> dashAttack; this -> combo_2(������ӻ�)
                            if (possibility < 0.3f)
                            {
                                //this -> dashAttack(������ͻ������): 30%����ͻ��
                                enemyManager.actionState.actionAnimationName = Dash_Attack;
                                ChangeState(enemyManager.actionState);
                            }
                            else
                            {
                                //this -> combo_2: 20%�����빥��
                                //this -> dash_F: 30%��ǰ����
                                //this -> roll_F: 20%��ǰ����
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
                    //TODO��������mid�л������ģ�Ҳ�����Ǵ�max����mid��һ˲���л�������,��ӷ�����
                    //������mid�л������Ļ���max->mid��һ˲���л������ģ������������̬��preventDrawedTimer��������
                    //�Ѿ��������úܾ��ˣ�����ͻ������Ϊ
                    if (preventDrawedTimer > maxDrawedTime)
                    {
                        //this -> combo_1(�о���ͻ������); this -> combo_3(�о���ͻ������);
                        if (possibility < 0.3f)
                        {
                            //this -> combo_1: 30%�о���ͻ������
                            enemyManager.actionState.actionAnimationName = Combo_1;
                            ChangeState(enemyManager.actionState);
                        }
                        else if (possibility < 0.8f)
                        {
                            //this -> combo_3: 50%�о���ͻ������
                            enemyManager.actionState.actionAnimationName = Combo_3;
                            ChangeState(enemyManager.actionState);
                        }
                        else
                        {
                            //this -> roll_F: 20%��ǰ����
                            enemyManager.actionState.actionAnimationName = Roll_F;
                            ChangeState(enemyManager.actionState);
                        }
                    }
                    break;
                case EnumManager.EnemyExplorePlayerDistanceType.Min:
                    //�����������ʱ
                    //this -> explore; this -> combo_4(�̾��빥��); this -> rollBack(��󷭹�); this -> combo_5(����������)
                    if (possibility < 0.1f)
                    {
                        //this -> explore: 10%��̽
                        ChangeState(enemyManager.exploreState);
                    }
                    else if (possibility < 0.5f)
                    {
                        //this -> combo_4: 40%�̾��빥��
                        enemyManager.actionState.actionAnimationName = Combo_4;
                        ChangeState(enemyManager.actionState);
                    }
                    else if (possibility < 0.7f)
                    {
                        //this->combo_5: 20%�̾�������
                        enemyManager.actionState.actionAnimationName = Combo_5;
                        ChangeState(enemyManager.actionState);
                    }
                    else if (possibility < 0.9f)
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
                    //this -> run: ����Ѿ��뿪�˿���̽�ķ�Χ���ܹ�ȥ
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
