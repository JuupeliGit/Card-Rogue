using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack", menuName = "Ability/Attack")]
public class Attack : Ability
{
    public override void Activate()
    {
        PlayerStats.instance.Attack();
    }
}
