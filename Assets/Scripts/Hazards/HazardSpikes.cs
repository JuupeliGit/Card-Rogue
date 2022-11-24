using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spikes", menuName = "Hazard/Spikes")]
public class HazardSpikes : FloorHazard
{
    public override void OnStep()
    {
        PlayerStats.instance.ModifyHealth(-1, false);
    }
}
