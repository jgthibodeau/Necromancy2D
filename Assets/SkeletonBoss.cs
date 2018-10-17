using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonBoss : Enemy
{
    public PrefabSpawner spawner;

    private bool attacking = false;

    void Update()
    {
        if (attacking)
        {
            spawner.active = true;
            attacking = false;
        } else
        {
            spawner.active = false;
        }
    }

    public override void Attack()
    {
        base.Attack();
        attacking = true;
    }
}
