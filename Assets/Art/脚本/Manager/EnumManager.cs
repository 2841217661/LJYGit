using UnityEngine;

public static class EnumManager {
    public enum PlayerAttackState
    {
        normal,
        attack,
    }

    //����ܻ�����
    public enum PlayerDamageType
    {
        None, // �޷�������
        Light, // �������
        Medium, // ���ز���Ĺ���
        Heavy, // �ػ�����
        FlyAway, //�����ɵĹ���
        AirLight, //�ڿ��б����
    }

    public enum EnemyAttackType
    {
        Normal,
        Attack,
    }

    public enum EnemyDamageType // �����ܻ�����
    {
        None, // ĳЩ�����޷�������
        Light, // �������
        Medium, // ���ز���Ĺ���
        Heavy, // �ػ�����
        FlyAway, //�����ɵĹ���
        AirLight, //�ڿ��б����
    }

    public enum EnemyWithPlayerDistanceType //�����������ҵľ�������
    {
        FarAway, //�����ǻ���Χ
        InWanderRange, //���ǻ���Χ�ڣ����Ҵ��ڹ�����Χ
        InAttackRange, //�ڹ�����Χ��
    }

    public enum EnemyExplorePlayerDistanceType
    {
        None, //��ʾû�е�����̽����
        Max,
        Med,
        Min,
    }

}
