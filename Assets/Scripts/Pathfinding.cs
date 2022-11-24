using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{
    private Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

    private PathNode[,] nodes;

    public void FindPath(Vector2Int startPos, Vector2Int endPos)
    {
        Vector2Int size = Dungeon.instance.size;
        nodes = new PathNode[size.x, size.y];

        nodes[startPos.x, startPos.y] = new PathNode(startPos, endPos, null);

        for(int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {

            }
        }
    }

    /*
    private void ShuffleDirections()
    {
        for (int i = 0; i < directions.Length; i++)
        {
            Vector2Int temp = directions[i];
            int r = Random.Range(i, directions.Length);
            directions[i] = directions[r];
            directions[r] = temp;
        }
    }
    */
}
