using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform player;  // 拖拽玩家对象到Inspector中的这个字段
    private NavMeshAgent agent;
    public float stoppingDistance = 1.5f; // 停止距离，可根据需要调整

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        // 设置移动参数（可选）
        agent.speed = 3.5f;    // 移动速度
        agent.angularSpeed = 120f;  // 旋转速度
        agent.acceleration = 8f;    // 加速度
        agent.stoppingDistance = stoppingDistance; // 设置代理的停止距离
    }

    void Update()
    {
        if (player != null)
        {
            // 计算与玩家的距离
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            // 如果距离大于停止距离，继续导航
            if (distanceToPlayer > stoppingDistance)
            {
                agent.isStopped = false; // 确保代理没有停止
                agent.SetDestination(player.position);
            }
            else
            {
                // 距离足够近时停止移动
                agent.isStopped = true;

                // 这里可以添加攻击逻辑或其他行为
                // 例如：transform.LookAt(player.position); // 保持面向玩家
            }
        }
    }
}