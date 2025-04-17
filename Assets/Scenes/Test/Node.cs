using UnityEngine;

public class Node
{
    public bool isWalkable;
    public Vector3 worldPosition;
    public int x, y, z;

    public int gCost; // 起点到该节点的消耗
    public int hCost; // 该节点到目标的估值
    public Node parent; // 用于回溯路径

    public int fCost => gCost + hCost;

    public Node(bool walkable, Vector3 worldPos, int x, int y, int z)
    {
        isWalkable = walkable;
        worldPosition = worldPos;
        this.x = x;
        this.y = y;
        this.z = z;
    }
}

