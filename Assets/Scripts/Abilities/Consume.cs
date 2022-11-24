using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Consume", menuName = "Ability/Consume")]
public class Consume : Ability
{
    public override void Activate()
    {
        ActionManager.instance.consumeCard = true;
        ActionManager.instance.actionFinished = true;
    }
}
