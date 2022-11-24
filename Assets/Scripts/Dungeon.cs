using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Dungeon : MonoBehaviour
{
    public static Dungeon instance;

    public Vector2Int size;
    public Tilemap wallTilemap;
    public Tilemap hazardTilemap;

    public Tile[] tiles;

    public int[,] tileData;
    public Hazard[,] hazardMap;

    private Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

    int dungeonStyle = 1;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);
    }

    public void GenerateDungeon()
    {
        dungeonStyle = Random.Range(2, 5);

        tileData = new int[size.x, size.y];
        hazardMap = new Hazard[size.x, size.y];

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                // Generate outer walls.
                if (x <= 0 || x >= size.x - 1 || y <= 0 || y >= size.y - 1)
                    tileData[x, y] = dungeonStyle;
            }
        }

        for(int i = 0; i < 10; i++)
        {
            tileData[GetRandomEmptyTile().x, GetRandomEmptyTile().y] = dungeonStyle;
        }

        RemoveDeadEnds();

        SetTiles();

        EntityManager.instance.Initialize();
        PlayerStats.instance.RandomizePlayerPosition();
    }

    private void RemoveDeadEnds()
    {
        for (int x = 1; x < size.x - 1; x++)
        {
            for (int y = 1; y < size.y - 1; y++)
            {
                if (tileData[x, y] != 0)
                {
                    int neighbors = 0;

                    for (int bx = -1; bx < 2; bx++)
                    {
                        for (int by = -1; by < 2; by++)
                        {
                            Vector2Int checkPos = new Vector2Int(x + bx, y + by);

                            if (!IndexOutOfRange(checkPos) && tileData[checkPos.x, checkPos.y] != 0)
                            {
                                neighbors++;
                            }
                        }
                    }

                    if (neighbors > 3)
                    {
                        tileData[x, y] = 0;
                        x = 1;
                        y = 1;
                    }
                }
            }
        }
    }

    private void SetTiles()
    {
        wallTilemap.ClearAllTiles();

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                int i = tileData[x, y];
                wallTilemap.SetTile(new Vector3Int(x, y, 0), tiles[i]);
                hazardTilemap.SetTile(new Vector3Int(x, y, 0), null);
            }
        }
    }

    public void SpawnStairs()
    {
        Vector2Int pos = EntityManager.instance.GetRandomEmptyTile();

        Vector3 particlePos = new Vector3(pos.x - 4.5f, pos.y - 2.5f, 0f);
        EntityManager.instance.SpawnSmokeParticlesAt(particlePos);

        wallTilemap.SetTile(new Vector3Int(pos.x, pos.y, 0), tiles[1]);
    }

    public Vector2Int GetRandomEmptyTile()
    {
        Vector2Int pos = Vector2Int.zero;

        do
        {
            pos.x = Random.Range(1, size.x - 1);
            pos.y = Random.Range(1, size.y - 1);
        }
        while (tileData[pos.x, pos.y] != 0);

        return pos;
    }

    public int GetTileAtPosition(Vector2Int pos, bool convert)
    {
        if (convert)
            pos = new Vector2Int(pos.x + 5, pos.y + 3);

        if (!IndexOutOfRange(pos))
            return tileData[pos.x, pos.y];
        else
            return -1;
    }

    public bool IndexOutOfRange(Vector2Int pos)
    {
        return (pos.x < 0 || pos.x > size.x - 1 || pos.y < 0 || pos.y > size.y - 1);
    }

    public void SetHazardAtPosition(Vector2Int pos, bool convert, Hazard hazard)
    {
        if (convert)
            pos = new Vector2Int(pos.x + 5, pos.y + 3);

        if (!IndexOutOfRange(pos))
        {
            hazardMap[pos.x, pos.y] = hazard;

            Tile newTile = hazard == null ? null : hazard.stats.hazardTile;
            hazardTilemap.SetTile(new Vector3Int(pos.x, pos.y, 0), newTile);
        }
    }

    public Hazard GetHazardAtPosition(Vector2Int pos, bool convert)
    {
        if (convert)
            pos = new Vector2Int(pos.x + 5, pos.y + 3);

        if (!IndexOutOfRange(pos))
            return hazardMap[pos.x, pos.y];
        else
            return null;
    }
}
