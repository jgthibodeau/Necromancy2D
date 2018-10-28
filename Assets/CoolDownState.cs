using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoolDownState : FSMState
{
    protected override StateID GetID() { return StateID.CoolDown; }

    public override void Reason(GameObject npc)
    {
    }

    public override void Act(GameObject npc)
    {
    }
}
