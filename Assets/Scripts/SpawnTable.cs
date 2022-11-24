using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct EntitySlot
{
    public EntityBaseStats entityStats;
    public float weight;
}

[CreateAssetMenu(fileName = "New Spawn Table", menuName = "Table/Spawn Table")]
public class SpawnTable : ScriptableObject
{
    public EntitySlot[] entitites;

    public EntityBaseStats GetRandomEntityStats()
    {
        float totalWeight = 0f;

        for(int i = 0; i < entitites.Length; i++)
        {
            totalWeight += entitites[i].weight;
        }

        float r = Random.Range(0f, totalWeight);
        float currentTotal = 0f;

        for (int i = 0; i < entitites.Length; i++)
        {
            currentTotal += entitites[i].weight;
            if (r < currentTotal)
                return entitites[i].entityStats;
        }

        return entitites[0].entityStats;
    }
}
