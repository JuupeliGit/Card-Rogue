using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Entity", menuName = "Entity")]
public class EntityBaseStats : ScriptableObject
{
    [Header("Sprite")]
    public Sprite sprite;
    public Color spriteColor;

    [Header("Stats")]
    public int maxHealth;
    public int stepsPerTurn;
    public int attacksPerTurn;

    [Header("Hazard")]
    public FloorHazard hazard;

    [Header("Status Condition")]
    public StatusCondition statusCondition;
    [Range(0f, 1f)] public float statusChance;
    public int statusTurns;

    [Header("Filler Cards")]
    public Card card;
    [Range(0f, 1f)] public float cardChance;

    [Header("Loot")]
    public LootTable loot;
}
