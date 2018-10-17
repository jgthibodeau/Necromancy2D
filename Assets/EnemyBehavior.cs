using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    public enum FOLLOW_STATE { FAR, MID, CLOSE }
    public FOLLOW_STATE followState = FOLLOW_STATE.FAR;

    public Transform player;
    public Health playerHealth;

    public Enemy enemy;
    public Transform target;

    public float minDistance;
    public float desiredDistance;
    public float maxDistance;
    public float attackDistance;

    private EntityController controller;
    private AiController aiController;

    public float attackRate;
    public float attackDelay;

    public bool stayAlerted;
    public float visionRadius, alertedVisionRadius;
    public bool alerted;

    public float minChangeDirTime, maxChangeDirTime;
    public float currentChangeDirTime;

    public float wanderSpeed, chaseSpeed;
    public float wanderTurnSpeed, chaseTurnSpeed;
    public float minWanderChangeTime, maxWanderChangeTime;
    private float currentWanderChangeTime;


    private float distanceToTarget = Mathf.Infinity;

    void Awake()
    {
        controller = GetComponent<EntityController>();
        aiController = GetComponent<AiController>();
    }

    void Start()
    {
        player = MyGameManager.instance.GetPlayer().transform;
        playerHealth = player.GetComponentInChildren<Health>();
        distanceToTarget = Mathf.Infinity;
    }

    public void DoBehavior()
    {
        controller.active = true;

        CheckForTarget();

        if (attackDelay > 0)
        {
            attackDelay -= Time.fixedDeltaTime;
        }

        if (alerted)
        {
            DebugExtension.DebugCircle(transform.position, Vector3.forward, Color.red, alertedVisionRadius);
            DebugExtension.DebugCircle(transform.position, Vector3.forward, Color.yellow, visionRadius);
            ChaseTarget();
        }
        else
        {
            DebugExtension.DebugCircle(transform.position, Vector3.forward, Color.red, alertedVisionRadius);
            DebugExtension.DebugCircle(transform.position, Vector3.forward, Color.yellow, visionRadius);
            Wander();
        }
        
        controller.RotateTowardsMotion();
    }

    void CheckForTarget()
    {
        if (playerHealth == null || playerHealth.IsDead())
        {
            alerted = false;
            return;
        }

        AquireTarget();

        distanceToTarget = Vector3.Distance(target.position, transform.position);
        if (alerted && !stayAlerted && distanceToTarget > alertedVisionRadius)
        {
            alerted = false;
        }
        else if (!alerted && distanceToTarget < visionRadius)
        {
            alerted = true;
        }
    }

    void AquireTarget()
    {
        //raycast to player
        //closest entity becomes target
        Vector2 direction = player.position - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, direction.magnitude, enemy.aliveTargetLayers);
        target = hit.transform;
    }

    void ChaseTarget()
    {
        aiController.targetTransform = target;
        aiController.minDistance = minDistance;
        aiController.maxDistance = maxDistance;
        aiController.desiredDistance = desiredDistance;

        controller.speed = chaseSpeed;
        controller.turnSpeed = chaseTurnSpeed;
        controller.RotateTowards(target);

        if (aiController.distanceToTarget < attackDistance)
        {
            Attack();
        }

        //Vector2 playerTarget = target.position;

        //Debug.DrawRay(transform.position, (playerTarget - (Vector2)transform.position), Color.red);
        //float distanceToTarget = Vector3.Distance(target.transform.position, transform.position);

        //Vector2 direction = playerTarget - (Vector2)transform.position;
        //controller.active = true;

        //if (distanceToTarget > maxDistance)
        //{
        //    followState = FOLLOW_STATE.FAR;
        //}
        //else if (distanceToTarget > minDistance)
        //{
        //    followState = FOLLOW_STATE.MID;
        //}
        //else if (distanceToTarget < minDistance)
        //{
        //    followState = FOLLOW_STATE.CLOSE;
        //}

        //switch (followState)
        //{
        //    case FOLLOW_STATE.FAR:
        //        controller.speed = chaseSpeed;
        //        moveDirection = direction;
        //        break;
        //    case FOLLOW_STATE.MID:
        //        controller.speed = chaseSpeed;
        //        moveDirection = direction;

        //        float scale = 1f;
        //        if (distanceToTarget > desiredDistance) {
        //            scale = Util.ConvertScale(desiredDistance, maxDistance, 0, 1, distanceToTarget);
        //        } else
        //        {
        //            scale = -Util.ConvertScale(minDistance, desiredDistance, 0, 1, distanceToTarget);
        //        }
        //        if (distanceToTarget <= attackDistance)
        //        {
        //            Attack();
        //        }
        //        //moveDirection *= scale;
        //        controller.speed *= scale;
        //        break;
        //    case FOLLOW_STATE.CLOSE:
        //        controller.speed = chaseSpeed;
        //        moveDirection = -direction;
        //        break;
        //}

        //moveDirection = enemy.Avoidance(moveDirection);

        //controller.turnSpeed = chaseTurnSpeed;
        //controller.SetMoveDirection(moveDirection);
        //controller.RotateTowards(target);
    }

    void Attack()
    {
        if (attackDelay <= 0)
        {
            attackDelay = attackRate;
            enemy.Attack();
        }
    }

    void Wander()
    {
        if (currentWanderChangeTime > 0)
        {
            currentWanderChangeTime -= Time.fixedDeltaTime;
        } else
        {
            currentWanderChangeTime = Random.Range(minWanderChangeTime, maxWanderChangeTime);
            controller.SetMoveDirection(Random.insideUnitCircle.normalized);
        }

        aiController.targetTransform = null;

        controller.speed = wanderSpeed;
        controller.turnSpeed = wanderTurnSpeed;
        controller.RotateTowardsMotion();
    }
}
