using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    Vector2Int currentPos;

    public float fCost;
    public float gCost;
    public float hCost;

    public PathNode cameFrom;

    public PathNode(Vector2Int currentPos, Vector2Int targetPos, PathNode cameFrom)
    {
        this.currentPos = currentPos;

        gCost = cameFrom == null ? 0f : cameFrom.gCost + 1f;
        hCost = Vector2.Distance(currentPos, targetPos);
        fCost = gCost + hCost;

        this.cameFrom = cameFrom;
    }
}
