using UnityEngine;

using System.Collections;

public class PathFollower : MonoBehaviour
{
    public float speed = 2f;
    private Grid3D grid;

    void Start()
    {
        grid = FindFirstObjectByType<Grid3D>();
        StartCoroutine(FollowPath());
    }

    IEnumerator FollowPath()
    {
        while (grid.path == null || grid.path.Count == 0)
            yield return null;

        foreach (Node node in grid.path)
        {
            Vector3 targetPos = node.worldPosition;
            while (Vector3.Distance(transform.position, targetPos) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
                yield return null;
            }
        }
    }
}

