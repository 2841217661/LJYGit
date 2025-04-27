using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 3D 网格生成器：在场景中生成一个规则的立方体网格，用于寻路或障碍检测
/// </summary>
public class Grid3D : MonoBehaviour
{
    // 网格覆盖的世界空间大小（宽、高、深）
    public Vector3 gridWorldSize = new Vector3(20, 5, 20);

    // 每个节点的半径，通过 Inspector 中的滑动条调整，范围 0.1 - 5
    [Range(0.1f, 5f)]
    public float nodeRadius = 0.5f;

    // 用于检测哪些区域是“不可行走”的，指定为某个 Layer（如 Obstacle）
    public LayerMask unwalkableMask;

    // 是否在 Scene 视图中显示 Gizmos 可视化网格
    public bool displayGridGizmos = true;

    // 网格节点数据存储
    private Node[,,] grid;

    // 每个节点的直径（= nodeRadius * 2）
    private float nodeDiameter;

    // 网格在 X、Y、Z 方向的节点数
    private int gridSizeX, gridSizeY, gridSizeZ;


    /// <summary>
    /// 初始化网格（运行游戏时调用一次）
    /// </summary>
    void Start()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        gridSizeZ = Mathf.RoundToInt(gridWorldSize.z / nodeDiameter);

        CreateGrid(); // 创建网格节点并检测是否可行走
    }


    /// <summary>
    /// 构建三维网格，并判断每个节点是否可行走
    /// </summary>
    /// 
    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY, gridSizeZ];

        // 计算整个网格的左下后角的世界位置（起始点）
        Vector3 worldBottomLeft = transform.position -
            Vector3.right * gridWorldSize.x / 2 -
            Vector3.forward * gridWorldSize.z / 2 -
            Vector3.up * gridWorldSize.y / 2;

        // 遍历网格中的每个节点
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                for (int z = 0; z < gridSizeZ; z++)
                {
                    // 计算当前节点的世界坐标位置
                    Vector3 worldPoint = worldBottomLeft +
                        Vector3.right * (x * nodeDiameter + nodeRadius) +
                        Vector3.up * (y * nodeDiameter + nodeRadius) +
                        Vector3.forward * (z * nodeDiameter + nodeRadius);

                    // 使用 Box 检测判断当前位置是否有障碍物
                    bool walkable = !Physics.CheckBox(
                        worldPoint,
                        Vector3.one * nodeRadius * 0.9f, // 较小的包围盒避免误判边缘
                        Quaternion.identity,
                        unwalkableMask // 与哪些 Layer 碰撞来判断是否不可行走
                    );

                    // 存入节点数组
                    grid[x, y, z] = new Node(walkable, worldPoint, x, y, z);
                }
            }
        }
    }

    /// <summary>
    /// 在 Scene 视图中绘制 Gizmos（每帧调用一次）
    /// </summary>
    void OnDrawGizmos()
    {
        // 每次绘图时更新 nodeDiameter 和格子大小
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        gridSizeZ = Mathf.RoundToInt(gridWorldSize.z / nodeDiameter);

        // 重新生成网格（调试时每帧更新）
        CreateGrid();

        // 绘制整个网格的边框
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, gridWorldSize);

        // 绘制每个节点（红色表示障碍物，白色表示可行走）
        if (displayGridGizmos && grid != null)
        {
            foreach (Node node in grid)
            {
                Gizmos.color = node.isWalkable ? Color.white : Color.red;
                // 完全填满格子（无间隙）
                Gizmos.DrawCube(node.worldPosition, Vector3.one * nodeDiameter);

            }
        }
    }

    public List<Node> path;

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = Mathf.Clamp01((worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x);
        float percentY = Mathf.Clamp01((worldPosition.y + gridWorldSize.y / 2) / gridWorldSize.y);
        float percentZ = Mathf.Clamp01((worldPosition.z + gridWorldSize.z / 2) / gridWorldSize.z);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        int z = Mathf.RoundToInt((gridSizeZ - 1) * percentZ);

        return grid[x, y, z];
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    if (x == 0 && y == 0 && z == 0)
                        continue;

                    int checkX = node.x + x;
                    int checkY = node.y + y;
                    int checkZ = node.z + z;

                    if (checkX >= 0 && checkX < gridSizeX &&
                        checkY >= 0 && checkY < gridSizeY &&
                        checkZ >= 0 && checkZ < gridSizeZ)
                    {
                        neighbours.Add(grid[checkX, checkY, checkZ]);
                    }
                }
            }
        }

        return neighbours;
    }

}
