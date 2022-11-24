using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusCondition : ScriptableObject
{
    public Sprite icon;
    public GameObject conditionUIPrefab;

    [TextArea] public string toolTipText;

    public virtual void EnableStatus()
    {

    }

    public virtual void DisableStatus()
    {

    }
}
