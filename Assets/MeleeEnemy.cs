using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : Enemy {
    public int damage;
    public WeaponCollider weaponCollider;
    public WeaponCollider allyWeaponCollider;

    void Update()
    {
        if (lifeState == LIFE_STATE.DEAD)
        {
            StopAttack();
        }
    }

    public override void StartAttack()
    {
        if (lifeState == LIFE_STATE.ALIVE)
        {
            weaponCollider.Activate(damage, currentTargetLayers);
        }
        if (lifeState == LIFE_STATE.RESURECTED)
        {
            allyWeaponCollider.Activate(damage, currentTargetLayers);
        }
    }

    public override void StopAttack()
    {
        weaponCollider.DeActivate();
        allyWeaponCollider.DeActivate();
    }
}
