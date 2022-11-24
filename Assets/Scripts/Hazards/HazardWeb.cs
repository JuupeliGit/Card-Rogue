using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Web", menuName = "Hazard/Web")]
public class HazardWeb : FloorHazard
{
    public StatusCondition condition;
    public int turns;

    public override void OnStep()
    {
        Status status = new Status(condition, turns);

        PlayerStats.instance.ApplyStatusCondition(status);
    }
}
