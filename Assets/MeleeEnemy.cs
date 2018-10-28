using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : Enemy {
    public int damage;
    public WeaponCollider weaponCollider;
    public WeaponCollider allyWeaponCollider;

    void Update()
    {
        //if (lifeState == LIFE_STATE.DEAD)
        if (fsm.CurrentStateID == StateID.Dead)
        {
            StopAttack();
        }
    }

    public override void StartAttack()
    {
        StopAttack();
        //if (lifeState == LIFE_STATE.ALIVE)
        if (fsm.CurrentStateID == StateID.Chase)
        {
            weaponCollider.Activate(damage, currentTargetLayers);
        }
        //if (lifeState == LIFE_STATE.RESURECTED)
        if (fsm.CurrentStateID == StateID.Ally)
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
