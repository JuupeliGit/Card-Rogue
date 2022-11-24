using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Draw", menuName = "Ability/Draw")]
public class Draw : Ability
{
    public override void Activate()
    {
        Deck.instance.DrawCard();
    }
}
