using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bleed", menuName = "Status Condition/Bleed")]
public class StatusBleed : StatusCondition
{
    public override void EnableStatus()
    {
        PlayerStats.instance.event_OnStep += BleedDamage;
    }

    public override void DisableStatus()
    {
        PlayerStats.instance.event_OnStep -= BleedDamage;
    }

    public void BleedDamage()
    {
        PlayerStats.instance.ModifyHealth(-1, false);
    }
}
