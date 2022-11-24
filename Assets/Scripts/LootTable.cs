using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct LootSlot
{
    public Card card;
    public float weight;
}

[CreateAssetMenu(fileName = "New Loot Table", menuName = "Table/Loot Table")]
public class LootTable : ScriptableObject
{
    public LootSlot[] loot;

    public Card GetRandomLoot()
    {
        float totalWeight = 0f;

        for (int i = 0; i < loot.Length; i++)
        {
            totalWeight += loot[i].weight;
        }

        float r = Random.Range(0f, totalWeight);
        float currentTotal = 0f;

        for (int i = 0; i < loot.Length; i++)
        {
            currentTotal += loot[i].weight;
            if (r < currentTotal)
                return loot[i].card;
        }

        return loot[0].card;
    }
}
