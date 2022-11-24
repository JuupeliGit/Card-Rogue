using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Curse", menuName = "Status Condition/Curse")]
public class StatusCursed : StatusCondition
{
    public override void EnableStatus()
    {
        Deck.instance.event_OnResolve += Curse;
    }

    public override void DisableStatus()
    {
        Deck.instance.event_OnResolve -= Curse;
    }

    public void Curse()
    {
        PlayerStats.instance.ModifyHealth(-1, false);
    }
}
