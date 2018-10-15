using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class AllyBehavior : MonoBehaviour
{
    public enum FOLLOW_STATE { FAR, MID, CLOSE }
    public FOLLOW_STATE followState = FOLLOW_STATE.FAR;

    public Transform player;
    public SummonSpot summonSpot;
    public Enemy enemy;
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
    }

    void Start()
    {
        player = MyGameManager.instance.GetPlayer().transform;
    }

    public void DoBehavior()
    {
        aiController.targetTransform = summonSpot.transform;
        aiController.minDistance = minDistance;
        aiController.maxDistance = maxDistance;
        aiController.desiredDistance = minDistance;

        entityController.turnSpeed = turnSpeed;
        entityController.RotateAwayFrom(player);

        if (attacking)
        {
            entityController.speed = attackMoveSpeed;
        }
        else
        {
            entityController.speed = speed;
        }
    }
}
