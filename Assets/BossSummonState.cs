using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSummonState : FSMState
{
    protected override StateID GetID() { return StateID.BossSummon; }

    public float turnSpeed;

    public PrefabSpawner spawner;
    public float spawnTime;
    private float currentSpawnTime;

    public int minSpawnCount, maxSpawnCount;
    public bool done;

    private int spawnCount;

    private Enemy enemy;
    private EntityController controller;
    private Transform player;

    void Start()
    {
        enemy = GetComponent<Enemy>();
        controller = GetComponent<EntityController>();
        player = MyGameManager.instance.GetPlayer().transform;
    }

    public override void DoBeforeEntering()
    {
        base.DoBeforeEntering();
        spawnCount = Random.Range(minSpawnCount, maxSpawnCount);
        currentSpawnTime = spawnTime;
        done = false;
        controller.turnSpeed = turnSpeed;
    }

    public override void DoBeforeLeaving()
    {
        base.DoBeforeLeaving();
    }

    public override void Reason(GameObject npc)
    {
        if (spawnCount <= 0)
        {
            enemy.SetTransition(Transition.SawPlayer);
        }
    }

    public override void Act(GameObject npc)
    {
        controller.Stop();
        controller.RotateTowards(player);

        if (spawnCount > 0)
        {
            if (currentSpawnTime > 0)
            {
                currentSpawnTime -= Time.fixedDeltaTime;
            }
            else
            {
                spawnCount--;
                Spawn();
            }
        }
    }

    void Spawn()
    {
        enemy.currentAnimator.SetTrigger("Summon");

        if (spawner.SpawnCount() < spawner.maxToSpawn)
        {
            spawner.Spawn(true);
            currentSpawnTime = spawnTime;
            spawnCount--;
        } else
        {
            spawnCount = 0;
        }
    }
}
