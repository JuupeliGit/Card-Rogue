using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Discard", menuName = "Ability/Discard")]
public class Discard : Ability
{
    public override void Activate()
    {
        Deck.instance.DiscardRandom();
    }
}
