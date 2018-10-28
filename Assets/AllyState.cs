using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyState : FSMState
{
    protected override StateID GetID() { return StateID.Ally; }

    private enum FOLLOW_STATE { FAR, MID, CLOSE }
    private FOLLOW_STATE followState = FOLLOW_STATE.FAR;

    private Transform player;
    private SummonSpot summonSpot;
    private Enemy enemy;
    private EntityController entityController;
    private AiController aiController;

    public float minDistance;
    public float maxDistance;
    public float speedScaleDistance;

    public float speed;
    public float turnSpeed;

    public bool attacking;
    public float attackMoveSpeed;
    
    void Awake()
    {
        entityController = GetComponent<EntityController>();
        aiController = GetComponent<AiController>();
        enemy = GetComponent<Enemy>();
    }

    void Start()
    {
        player = MyGameManager.instance.GetPlayer().transform;
    }

    public override void Reason(GameObject npc)
    {
        //// If the player has gone 30 meters away from the NPC, fire LostPlayer transition
        //if (Vector3.Distance(npc.transform.position, player.transform.position) >= 30)
        //    npc.GetComponent<EnemyController>().SetTransition(Transition.LostPlayer);
    }

    public override void Act(GameObject npc)
    {
        if (summonSpot != null)
        {
            aiController.targetTransform = summonSpot.transform;
        }
        aiController.minDistance = minDistance;
        aiController.maxDistance = maxDistance;
        aiController.desiredDistance = minDistance;

        entityController.turnSpeed = turnSpeed;
        entityController.RotateAwayFrom(player);

        //if (attacking)
        //{
        //    entityController.speed = attackMoveSpeed;
        //}
        //else
        //{
        //    entityController.speed = speed;
        //}
        entityController.speed = speed;
    }

    public void SetSummonSpot(SummonSpot newSpot)
    {
        if (summonSpot != null)
        {
            summonSpot.SetEnemy(null);
        }
        summonSpot = newSpot;
    }

    public void ResetSummonSpot()
    {
        if (summonSpot != null)
        {
            summonSpot.enemy = null;
            summonSpot = null;
        }
    }
}