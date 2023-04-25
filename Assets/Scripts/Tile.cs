using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Tile : PoolableObject
{
    public override void OnDisable()
    {
        base.OnDisable();
        this.gameObject.name = $"Tile (Clone)";
    }
}
