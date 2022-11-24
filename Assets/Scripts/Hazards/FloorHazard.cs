using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Empty", menuName = "Hazard/Empty")]
public class FloorHazard : ScriptableObject
{
    public Tile hazardTile;

    public virtual void OnStep()
    {

    }
}
