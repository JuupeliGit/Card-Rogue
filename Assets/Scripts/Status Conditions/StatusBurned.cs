using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Burn", menuName = "Status Condition/Burn")]
public class StatusBurned : StatusCondition
{
    public override void EnableStatus()
    {
        TurnManager.instance.event_OnTurnStart += Burn;
    }

    public override void DisableStatus()
    {
        TurnManager.instance.event_OnTurnStart -= Burn;
    }

    public void Burn()
    {
        Deck.instance.DiscardRandom();
    }
}
