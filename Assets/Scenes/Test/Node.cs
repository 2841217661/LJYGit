using UnityEngine;

public class Node
{
    public bool isWalkable;
    public Vector3 worldPosition;
    public int x, y, z;

    public int gCost; // ��㵽�ýڵ������
    public int hCost; // �ýڵ㵽Ŀ��Ĺ�ֵ
    public Node parent; // ���ڻ���·��

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

