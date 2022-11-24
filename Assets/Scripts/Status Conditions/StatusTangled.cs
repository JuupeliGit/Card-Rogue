using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tangled", menuName = "Status Condition/Tangled")]
public class StatusTangled : StatusCondition
{
    public override void EnableStatus()
    {
        PlayerStats.instance.event_CheckMove += Tangled;
    }

    public override void DisableStatus()
    {
        PlayerStats.instance.event_CheckMove -= Tangled;
    }

    public bool Tangled()
    {
        return false;
    }
}
