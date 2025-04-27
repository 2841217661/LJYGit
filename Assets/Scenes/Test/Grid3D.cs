using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 3D �������������ڳ���������һ���������������������Ѱ·���ϰ����
/// </summary>
public class Grid3D : MonoBehaviour
{
    // ���񸲸ǵ�����ռ��С�����ߡ��
    public Vector3 gridWorldSize = new Vector3(20, 5, 20);

    // ÿ���ڵ�İ뾶��ͨ�� Inspector �еĻ�������������Χ 0.1 - 5
    [Range(0.1f, 5f)]
    public float nodeRadius = 0.5f;

    // ���ڼ����Щ�����ǡ��������ߡ��ģ�ָ��Ϊĳ�� Layer���� Obstacle��
    public LayerMask unwalkableMask;

    // �Ƿ��� Scene ��ͼ����ʾ Gizmos ���ӻ�����
    public bool displayGridGizmos = true;

    // ����ڵ����ݴ洢
    private Node[,,] grid;

    // ÿ���ڵ��ֱ����= nodeRadius * 2��
    private float nodeDiameter;

    // ������ X��Y��Z ����Ľڵ���
    private int gridSizeX, gridSizeY, gridSizeZ;


    /// <summary>
    /// ��ʼ������������Ϸʱ����һ�Σ�
    /// </summary>
    void Start()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        gridSizeZ = Mathf.RoundToInt(gridWorldSize.z / nodeDiameter);

        CreateGrid(); // ��������ڵ㲢����Ƿ������
    }


    /// <summary>
    /// ������ά���񣬲��ж�ÿ���ڵ��Ƿ������
    /// </summary>
    /// 
    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY, gridSizeZ];

        // ����������������º�ǵ�����λ�ã���ʼ�㣩
        Vector3 worldBottomLeft = transform.position -
            Vector3.right * gridWorldSize.x / 2 -
            Vector3.forward * gridWorldSize.z / 2 -
            Vector3.up * gridWorldSize.y / 2;

        // ���������е�ÿ���ڵ�
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                for (int z = 0; z < gridSizeZ; z++)
                {
                    // ���㵱ǰ�ڵ����������λ��
                    Vector3 worldPoint = worldBottomLeft +
                        Vector3.right * (x * nodeDiameter + nodeRadius) +
                        Vector3.up * (y * nodeDiameter + nodeRadius) +
                        Vector3.forward * (z * nodeDiameter + nodeRadius);

                    // ʹ�� Box ����жϵ�ǰλ���Ƿ����ϰ���
                    bool walkable = !Physics.CheckBox(
                        worldPoint,
                        Vector3.one * nodeRadius * 0.9f, // ��С�İ�Χ�б������б�Ե
                        Quaternion.identity,
                        unwalkableMask // ����Щ Layer ��ײ���ж��Ƿ񲻿�����
                    );

                    // ����ڵ�����
                    grid[x, y, z] = new Node(walkable, worldPoint, x, y, z);
                }
            }
        }
    }

    /// <summary>
    /// �� Scene ��ͼ�л��� Gizmos��ÿ֡����һ�Σ�
    /// </summary>
    void OnDrawGizmos()
    {
        // ÿ�λ�ͼʱ���� nodeDiameter �͸��Ӵ�С
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        gridSizeZ = Mathf.RoundToInt(gridWorldSize.z / nodeDiameter);

        // �����������񣨵���ʱÿ֡���£�
        CreateGrid();

        // ������������ı߿�
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, gridWorldSize);

        // ����ÿ���ڵ㣨��ɫ��ʾ�ϰ����ɫ��ʾ�����ߣ�
        if (displayGridGizmos && grid != null)
        {
            foreach (Node node in grid)
            {
                Gizmos.color = node.isWalkable ? Color.white : Color.red;
                // ��ȫ�������ӣ��޼�϶��
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
