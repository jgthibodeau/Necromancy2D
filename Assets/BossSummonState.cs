using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSummonState : FSMState
{
    protected override StateID GetID() { return StateID.BossSummon; }

    public PrefabSpawner spawner;
    public float spawnTime;
    [SerializeField]
    private float currentSpawnTime;

    public override void DoBeforeEntering()
    {
        base.DoBeforeEntering();
        currentSpawnTime = spawnTime;
    }

    public override void DoBeforeLeaving()
    {
        base.DoBeforeLeaving();
        spawner.active = false;
    }

    public override void Reason(GameObject npc)
    {
        if (currentSpawnTime <= 0)
        {
            npc.GetComponent<Enemy>().SetTransition(Transition.SawPlayer);
        }
    }

    public override void Act(GameObject npc)
    {
        currentSpawnTime -= Time.deltaTime;
        spawner.active = true;
    }
}
