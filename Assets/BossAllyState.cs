using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAllyState : FSMState
{
    protected override StateID GetID() { return StateID.Ally; }

    public override void Reason(GameObject npc)
    {
    }

    public override void Act(GameObject npc)
    {
    }
}
