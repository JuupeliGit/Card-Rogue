using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    public static EntityManager instance;

    public List<Entity> entities = new List<Entity>();

    private Vector2Int size;
    public int[,] entityMap;

    public GameObject entityPrefab;
    public List<GameObject> entitiesInUse = new List<GameObject>();
    public List<GameObject> entitiesInReserve = new List<GameObject>();

    public EntityBaseStats rat;
    public SpawnTable[] spawnTables;
    public SpawnTable containerTable;

    public int currentId;
    public int enemiesLeft;
    public bool isAnimating = false;

    public delegate void OnEndSpriteAnimation();
    public event OnEndSpriteAnimation onEndSpriteAnimation;

    public GameObject hitParticles;
    public GameObject smokeParticles;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);

        for (int i = 0; i < 20; i++)
        {
            GameObject newCard = (GameObject)Instantiate(entityPrefab, transform);
            entitiesInReserve.Add(newCard);
        }
    }

    public void Initialize()
    {
        size = Dungeon.instance.size;
        entityMap = new int[size.x, size.y];

        for (int i = 0; i < entitiesInUse.Count; i++)
            entitiesInUse[i].SetActive(false);

        entities.Clear();

        entitiesInReserve.AddRange(entitiesInUse);
        entitiesInUse.Clear();

        currentId = 1;

        int enemyAmount = TurnManager.instance.floor > 1 ? Random.Range(1, 6) : 1;
        for (int i = 0; i < enemyAmount; i++)
            SpawnEnemy();

        enemiesLeft = entities.Count;

        int containerAmount = Random.Range(3, 6);
        for (int i = 0; i < containerAmount; i++)
            SpawnContainers();
    }

    private void SpawnEnemy()
    {
        GameObject entityObject = entitiesInReserve[entitiesInReserve.Count - 1];

        EntityBaseStats stats = rat;
        int floor = TurnManager.instance.floor;
        if (floor > 1)
        {
            int r = Mathf.Clamp(Random.Range(0, spawnTables.Length * 4), 0, floor) / 4;
            stats = spawnTables[r].GetRandomEntityStats();
        }

        Entity enemy = new Entity(currentId, stats, entityObject);
        entities.Add(enemy);

        entityMap[enemy.position.x, enemy.position.y] = enemy.id;

        entitiesInUse.Add(entityObject);
        entitiesInReserve.Remove(entityObject);

        entityObject.SetActive(true);

        currentId++;
    }

    private void SpawnContainers()
    {
        GameObject entityObject = entitiesInReserve[entitiesInReserve.Count - 1];

        Entity container = new Entity(currentId, containerTable.GetRandomEntityStats(), entityObject);
        entities.Add(container);

        entityMap[container.position.x, container.position.y] = container.id;

        entitiesInUse.Add(entityObject);
        entitiesInReserve.Remove(entityObject);

        entityObject.SetActive(true);

        currentId++;
    }

    public void RemoveEntity(Entity entity)
    {
        GameObject entityObject = entity.gameObject;

        entityMap[entity.position.x, entity.position.y] = 0;

        entities.Remove(entity);

        entitiesInReserve.Add(entityObject);
        entitiesInUse.Remove(entityObject);

        entityObject.SetActive(false);

        if (enemiesLeft <= 0)
            TurnManager.instance.EnableStairs();
    }

    public void StartEnemyTurn()
    {
        StartCoroutine(EnemyActions());
    }

    private IEnumerator EnemyActions()
    {
        for(int i = 0; i < entities.Count; i++)
        {
            Entity enemy = entities[i];

            // Walk
            for (int j = 0; j < enemy.stats.stepsPerTurn; j++)
            {
                yield return new WaitForEndOfFrame();

                isAnimating = true;

                entityMap[enemy.position.x, enemy.position.y] = 0;

                entities[i].Move();

                entityMap[enemy.position.x, enemy.position.y] = enemy.id;

                yield return new WaitUntil(() => !isAnimating);
            }
            // Attack
            for (int j = 0; j < enemy.stats.attacksPerTurn; j++)
            {
                yield return new WaitForEndOfFrame();

                isAnimating = true;

                entities[i].Attack();

                yield return new WaitUntil(() => !isAnimating);
            }

            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(0.1f);

        TurnManager.instance.EndEnemyTurn();
    }

    public IEnumerator MoveEntitySprite(GameObject target, Vector2 endPos)
    {
        float elapsed = 0f;
        float time = 0.1f;

        Vector2 startPos = target.transform.position;

        while(elapsed < time)
        {
            float posX = Mathf.Lerp(startPos.x, endPos.x, elapsed / time);
            float posY = Mathf.Lerp(startPos.y, endPos.y, elapsed / time);
            posY += Mathf.Clamp01(Mathf.Sin(elapsed / time) * Mathf.PI) * 0.3f;
            target.transform.position = new Vector2(posX, posY);

            elapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        target.transform.position = endPos;

        SoundManager.instance.PlaySound(4, 0.5f, 1f);

        yield return new WaitForSeconds(0.05f);

        isAnimating = false;
        onEndSpriteAnimation?.Invoke();
    }

    public IEnumerator AttackEntitySprite(GameObject target, Vector2 targetPos)
    {
        float elapsed = 0f;
        float time = 0.1f;

        Vector2 startPos = target.transform.position;

        while (elapsed < time)
        {
            float posX = Mathf.Lerp(startPos.x, targetPos.x, elapsed / time);
            float posY = Mathf.Lerp(startPos.y, targetPos.y, elapsed / time);
            posY += Mathf.Clamp01(Mathf.Sin(elapsed / time) * Mathf.PI) * 0.2f;
            target.transform.position = new Vector2(posX, posY);

            elapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        target.transform.position = targetPos;

        Instantiate(hitParticles, targetPos, Quaternion.identity);
        CameraEffects.instance.CameraShake(0.1f, 0.1f);

        yield return new WaitForSeconds(0.1f);

        elapsed = 0f;
        while (elapsed < time)
        {
            Vector2 curPos = Vector2.Lerp(targetPos, startPos, elapsed / time);
            target.transform.position = curPos;

            elapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        target.transform.position = startPos;

        yield return new WaitForSeconds(0.05f);

        isAnimating = false;
        onEndSpriteAnimation?.Invoke();
    }

    public void SpawnSmokeParticlesAt(Vector3 pos)
    {
        Instantiate(smokeParticles, pos, Quaternion.identity);
    }

    public Vector2Int GetRandomEmptyTile()
    {
        Vector2Int pos = Vector2Int.zero;

        do
        {
            pos.x = Random.Range(1, size.x - 1);
            pos.y = Random.Range(1, size.y - 1);
        }
        while (entityMap[pos.x, pos.y] != 0 || Dungeon.instance.tileData[pos.x, pos.y] != 0);

        return pos;
    }

    public int GetIdAtPosition(Vector2Int pos, bool convert)
    {
        if (convert)
            pos = new Vector2Int(pos.x + 5, pos.y + 3);

        if (!Dungeon.instance.IndexOutOfRange(pos))
            return entityMap[pos.x, pos.y];
        else
            return -1;
    }

    public Entity FindEntityById(int id)
    {
        foreach(Entity e in entities)
        {
            if (e.id == id)
                return e;
        }

        return null;
    }
}
