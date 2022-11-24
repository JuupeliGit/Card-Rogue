using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Poison", menuName = "Status Condition/Poison")]
public class StatusPoison : StatusCondition
{
    public override void EnableStatus()
    {
        TurnManager.instance.event_OnTurnEnd += PoisonDamage;
    }

    public override void DisableStatus()
    {
        TurnManager.instance.event_OnTurnEnd -= PoisonDamage;
    }

    public void PoisonDamage()
    {
        PlayerStats.instance.ModifyHealth(-1, false);
    }
}
