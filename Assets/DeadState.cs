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
        //// If the player has gone 30 meters away from the NPC, fire LostPlayer transition
        //if (Vector3.Distance(npc.transform.position, player.transform.position) >= 30)
        //    npc.GetComponent<EnemyController>().SetTransition(Transition.LostPlayer);
    }

    public override void Act(GameObject npc)
    {
        entityController.Stop();
        //// Follow the path of waypoints
        //// Find the direction of the player 		
        //Vector3 vel = npc.rigidbody.velocity;
        //Vector3 moveDir = player.transform.position - npc.transform.position;

        //// Rotate towards the waypoint
        //npc.transform.rotation = Quaternion.Slerp(npc.transform.rotation,
        //                                          Quaternion.LookRotation(moveDir),
        //                                          5 * Time.deltaTime);
        //npc.transform.eulerAngles = new Vector3(0, npc.transform.eulerAngles.y, 0);

        //vel = moveDir.normalized * 10;

        //// Apply the new Velocity
        //npc.rigidbody.velocity = vel;
    }
}
