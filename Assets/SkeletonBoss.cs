using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonBoss : Enemy
{
    public PrefabSpawner spawner;

    public override void StartAttack()
    {
        spawner.active = true;
    }

    public override void StopAttack()
    {
        spawner.active = false;
    }
}
