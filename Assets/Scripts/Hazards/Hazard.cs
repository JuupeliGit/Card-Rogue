using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard
{
    public FloorHazard stats;
    public int turnsLeft;

    public Hazard(FloorHazard hazard, int turnsLeft)
    {
        this.stats = hazard;
        this.turnsLeft = turnsLeft;
    }
}
