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
        //�����߼�
    }

    public virtual void Update()
    {
        // �����߼�
    }

    public virtual void Exit()
    {
        // �˳��߼�
    }

    public virtual void FixedUpdate()
    {
        // ��������߼�
    }


    public void ChangeState(EnemyState _newState)
    {
        enemyManager.currentState.Exit(); // ת����̬ʱ���õ�ǰ��̬���˳�����
        enemyManager.currentState = _newState; // ����ǰ����̬ת��Ϊ�µ���̬
        enemyManager.currentState.Enter(); // �����µ���̬�Ľ��뷽��
    }
}
