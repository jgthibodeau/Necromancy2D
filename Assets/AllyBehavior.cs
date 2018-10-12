using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyBehavior : MonoBehaviour
{
    public enum FOLLOW_STATE { FAR, MID, CLOSE }
    public FOLLOW_STATE followState = FOLLOW_STATE.FAR;

    public Transform player;
    public SummonSpot summonSpot;
    public Enemy enemy;
    private EntityController controller;

    public float minDistance;
    public float maxDistance;
    public float speedScaleDistance;

    public float speed;
    public float turnSpeed;

    public bool attacking;
    public float attackMoveSpeed;

    public float attackRate;
    private Vector2 moveDirection;

    void Start()
    {
        controller = GetComponent<EntityController>();
    }

    public void DoBehavior()
    {
        //move close to player
        FollowPlayer();

        //rotate away from player

        //if player clicked attack, attack
    }

    void FollowPlayer()
    {
        Vector2 target = summonSpot.transform.position;

        Debug.DrawRay(transform.position, (target - (Vector2)transform.position), Color.red);
        float distanceToTarget = Vector3.Distance(target, transform.position);

        Vector2 direction = target - (Vector2)transform.position;
        controller.active = true;

        if (distanceToTarget > maxDistance)
        {
            followState = FOLLOW_STATE.FAR;
        }
        else if (distanceToTarget > minDistance)
        {
            followState = FOLLOW_STATE.MID;
        }
        else// if (distanceToTarget <= minDistance)
        {
            followState = FOLLOW_STATE.CLOSE;
        }

        switch (followState)
        {
            case FOLLOW_STATE.FAR:
                controller.speed = speed;
                if (attacking)
                {
                    controller.speed = attackMoveSpeed;
                }
                moveDirection = direction;
                break;
            case FOLLOW_STATE.MID:
                controller.speed = speed;
                if (attacking)
                {
                    controller.speed = attackMoveSpeed;
                }
                moveDirection = direction;
                
                float scale = Util.ConvertScale(minDistance, maxDistance, 0, 1, distanceToTarget);
                //moveDirection *= scale;
                controller.speed *= scale;
                break;
            case FOLLOW_STATE.CLOSE:
                controller.Stop();
                controller.active = false;
                break;
        }

        moveDirection = enemy.Avoidance(moveDirection);

        controller.turnSpeed = turnSpeed;
        controller.SetMoveDirection(moveDirection);
        controller.RotateAwayFrom(player);
    }
}
