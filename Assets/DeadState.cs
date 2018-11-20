using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : FSMState
{
    protected override StateID GetID() { return StateID.Dead; }

    private EntityController entityController;

    void Awake()
    {
        entityController = GetComponent<EntityController>();
    }

    public override void Reason(GameObject npc)
    {
    }

    public override void Act(GameObject npc)
    {
        entityController.FullStop();
    }
}
