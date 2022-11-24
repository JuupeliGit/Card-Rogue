using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Slime", menuName = "Hazard/Slime")]
public class HazardSlime : FloorHazard
{
    public Card penaltyCard;

    public override void OnStep()
    {
        for (int i = 0; i < 2; i++)
            Deck.instance.AddNewCard(penaltyCard);
    }
}
