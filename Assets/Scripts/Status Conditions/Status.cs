using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Status
{
    public StatusCondition condition;
    public int turnsLeft;

    public GameObject conditionUI;

    public Status(StatusCondition condition, int turnsLeft)
    {
        this.condition = condition;
        this.turnsLeft = turnsLeft;
    }

    public void UpdateUI()
    {
        if (conditionUI != null)
            conditionUI.GetComponentInChildren<TMP_Text>().text = condition.name + " (" + turnsLeft +  (turnsLeft > 1 ? " turns)" : " turn)");
    }
}
