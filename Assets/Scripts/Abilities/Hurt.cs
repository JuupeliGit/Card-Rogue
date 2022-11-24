using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Hurt", menuName = "Ability/Hurt")]
public class Hurt : Ability
{
    public override void Activate()
    {
        PlayerStats.instance.ModifyHealth(-1, true);
    }
}
