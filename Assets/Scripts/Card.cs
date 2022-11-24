using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CardAbility
{
    public Ability ability;
    public int amount;
    public bool showAmount;
}

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Card : ScriptableObject
{
    public string cardName;
    public Sprite cardSprite;

    public CardAbility[] abilities;
}
