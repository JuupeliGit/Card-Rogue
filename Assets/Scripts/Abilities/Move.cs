using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Ability/Move")]
public class Move : Ability
{
    public override void Activate()
    {
        PlayerStats.instance.MovePlayer();
    }
}
