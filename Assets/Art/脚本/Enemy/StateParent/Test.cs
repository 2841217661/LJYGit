using UnityEngine;
using static EnumManager;

public class Test : MonoBehaviour
{
    public Transform testPoint1;
    public Transform testPoint2;
    public Transform testPoint3;
    public Transform testPoint4;
    public Transform testPoint5;
    public Transform testPoint6;
    public Transform testPoint7;
    public Transform testPoint8;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            RandomMoveToTestPoint();
        }
    }

    [Header("攻击检测")]
    public float attackRadius; //攻击检测范围
    public Vector3 attackOffsetPoint; //攻击检测圆心与自身位置的偏移量
    public LayerMask attackTargetLayer; //可以攻击的对象层级
    //攻击检测
    public bool AttackDetecte(int _attackType)
    {
        // 将整数转换为 DamageType
        PlayerDamageType damageType = _attackType switch
        {
            0 => PlayerDamageType.None,
            1 => PlayerDamageType.Light,  // 轻击
            2 => PlayerDamageType.Medium,   // 中等攻击
            3 => PlayerDamageType.Heavy,    // 重击
            4 => PlayerDamageType.FlyAway, //击飞
            5 => PlayerDamageType.AirLight, //空中轻击
            _ => PlayerDamageType.None, //默认无攻击类型
        };

        // 计算攻击偏移位置
        Vector3 offsetPoint = transform.rotation * attackOffsetPoint;

        Collider[] players = Physics.OverlapSphere(transform.position + offsetPoint, attackRadius, attackTargetLayer);

        if (players.Length == 0)
        {
            Debug.Log("无对象");
            return false;
        }

        foreach (Collider player in players)
        {
            PlayerManager playerManager = player.GetComponent<PlayerManager>();
            if (playerManager == null)
            {
                Debug.Log("空");
            }

            //获取面向玩家的方向向量
            Vector3 hitDir = playerManager.transform.position - transform.position;
            hitDir.y = 0;
            hitDir = hitDir.normalized;

            player.GetComponent<PlayerManager>().TakeDamage(hitDir, damageType);
        }

        return true;
    }

    public void RandomMoveToTestPoint()
    {
        // 把所有测试点放进一个数组
        Transform[] points = new Transform[]
        {
        testPoint1, testPoint2, testPoint3, testPoint4,
        testPoint5, testPoint6, testPoint7, testPoint8
        };

        // 随机选择一个索引
        int randomIndex = Random.Range(0, points.Length);

        // 检查这个点是否存在
        if (points[randomIndex] != null)
        {
            // 直接把当前位置设置到随机点的位置
            transform.position = points[randomIndex].position;

            Debug.Log("选择的点为:" + randomIndex);

            AttackDetecte(1);
        }
        else
        {
            Debug.LogWarning("随机选中的测试点为空！");
        }
    }

}
