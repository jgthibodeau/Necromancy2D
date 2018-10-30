using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoolDownState : FSMState
{
    protected override StateID GetID() { return StateID.CoolDown; }

    private Enemy enemy;
    private EntityController controller;
    private Transform player;

    public float coolDownTime;
    private float currentCoolDownTime;

    void Start()
    {
        enemy = GetComponent<Enemy>();
        controller = GetComponent<EntityController>();
        player = MyGameManager.instance.GetPlayer().transform;
    }

    public override void DoBeforeEntering()
    {
        base.DoBeforeEntering();
        currentCoolDownTime = coolDownTime;
    }

    public override void Reason(GameObject npc)
    {
        if (currentCoolDownTime < 0)
        {
            enemy.SetTransition(Transition.SawPlayer);
        }
    }

    public override void Act(GameObject npc)
    {
        controller.Stop();
        controller.RotateTowards(player);
    }
}
