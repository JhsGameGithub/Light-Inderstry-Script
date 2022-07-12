using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RawMaterial : Product
{
    protected override void Awake()
    {
        base.Awake();
        productLevel = 0;
        productScore = 0;
    }
}
