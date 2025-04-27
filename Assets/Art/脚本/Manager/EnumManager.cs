using UnityEngine;

public static class EnumManager {
    public enum PlayerAttackState
    {
        normal,
        attack,
    }

    //玩家受击类型
    public enum PlayerDamageType
    {
        None, // 无法被击退
        Light, // 轻击攻击
        Medium, // 不重不轻的攻击
        Heavy, // 重击攻击
        FlyAway, //被击飞的攻击
        AirLight, //在空中被轻击
    }

    public enum EnemyAttackType
    {
        Normal,
        Attack,
    }

    public enum EnemyDamageType // 敌人受击类型
    {
        None, // 某些敌人无法被击退
        Light, // 轻击攻击
        Medium, // 不重不轻的攻击
        Heavy, // 重击攻击
        FlyAway, //被击飞的攻击
        AirLight, //在空中被轻击
    }

    public enum EnemyWithPlayerDistanceType //敌人相对与玩家的距离类型
    {
        FarAway, //超过徘徊范围
        InWanderRange, //在徘徊范围内，并且大于攻击范围
        InAttackRange, //在攻击范围内
    }

    public enum EnemyExplorePlayerDistanceType
    {
        None, //表示没有到达试探距离
        Max,
        Med,
        Min,
    }

}
