using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Heal", menuName = "Ability/Heal")]
public class Heal : Ability
{
    public override void Activate()
    {
        PlayerStats.instance.ModifyHealth(1, true);
    }
}
