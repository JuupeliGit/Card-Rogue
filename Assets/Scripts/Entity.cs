using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity
{
    public int id;
    public EntityBaseStats stats;
    public GameObject gameObject;

    public int health;

    public Vector2Int position;

    private Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

    public Entity(int id, EntityBaseStats stats, GameObject gameObject)
    {
        this.id = id;
        this.stats = stats;
        this.gameObject = gameObject;

        SpriteRenderer rend = gameObject.GetComponent<SpriteRenderer>();
        rend.sprite = stats.sprite;
        rend.color = stats.spriteColor;

        health = stats.maxHealth;

        RandomizePosition();
    }

    private void RandomizePosition()
    {
        position = EntityManager.instance.GetRandomEmptyTile();
        UpdateSpritePosition();
    }

    public void ModifyHealth(int amount)
    {
        health += amount;

        if(amount < 0)
            SoundManager.instance.PlaySound(1, 0.5f, 1f);

        if (health > stats.maxHealth)
            health = stats.maxHealth;
        else if (health <= 0)
        {
            SoundManager.instance.PlaySound(2, 0.5f, 1f);

            Card loot = stats.loot.GetRandomLoot();
            if (loot != null)
                TurnManager.instance.DropLoot(loot);

            EntityManager.instance.SpawnSmokeParticlesAt(gameObject.transform.position);

            if (stats.attacksPerTurn > 0)
                EntityManager.instance.enemiesLeft--;

            EntityManager.instance.RemoveEntity(this);
        }
    }

    public void Act()
    {
        float distanceToPlayer = Vector2.Distance(position, PlayerStats.instance.playerPosition);

        if (distanceToPlayer > 1.25f)
            Move();
        else
            Attack();
    }

    public bool Move()
    {
        float distanceToPlayer = Vector2.Distance(position, PlayerStats.instance.playerPosition);
        if (distanceToPlayer <= 1.25f)
        {
            EntityManager.instance.isAnimating = false;
            return false;
        }

        ShuffleDirections();

        int closestDirection = -1;
        float closestDst = 100;

        for (int i = 0; i < directions.Length; i++)
        {
            Vector2Int dirPos = position + directions[i];
            distanceToPlayer = Vector2.Distance(dirPos, PlayerStats.instance.playerPosition);

            if (Dungeon.instance.GetTileAtPosition(dirPos, false) == 0 && EntityManager.instance.GetIdAtPosition(dirPos, false) == 0)
            {
                if (distanceToPlayer < closestDst)
                {
                    closestDst = distanceToPlayer;
                    closestDirection = i;
                }
            }
        }

        if (closestDirection != -1)
        {
            Vector2Int newPos = position + directions[closestDirection];

            if (Dungeon.instance.GetTileAtPosition(newPos, false) == 0 && EntityManager.instance.GetIdAtPosition(newPos, false) == 0)
            {
                if (stats.hazard != null)
                    Dungeon.instance.SetHazardAtPosition(position, false, new Hazard(stats.hazard, 2));

                position = newPos;
            }
        }

        UpdateSpritePosition();

        return true;
    }

    public bool Attack()
    {
        float distanceToPlayer = Vector2.Distance(position, PlayerStats.instance.playerPosition);
        if (distanceToPlayer > 1.25f)
        {
            EntityManager.instance.isAnimating = false;
            return false;
        }

        EntityManager.instance.StartCoroutine(EntityManager.instance.AttackEntitySprite(gameObject, PlayerStats.instance.playerSprite.position));

        PlayerStats.instance.ModifyHealth(-1, false);

        if (stats.statusCondition != null && Random.Range(0f, 1f) <= stats.statusChance)
            ApplyStatusCondition();

        if (stats.card != null && Random.Range(0f, 1f) <= stats.cardChance)
            Deck.instance.AddNewCard(stats.card);

        return true;
    }

    private void ApplyStatusCondition()
    {
        Status status = new Status(stats.statusCondition, stats.statusTurns);

        PlayerStats.instance.ApplyStatusCondition(status);
    }

    private void UpdateSpritePosition()
    {
        Vector2 pos = new Vector2(position.x - 5 + 0.5f, position.y - 3 + 0.5f);
        EntityManager.instance.StartCoroutine(EntityManager.instance.MoveEntitySprite(gameObject, pos));
    }

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
}
