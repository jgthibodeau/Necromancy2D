using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasePlayerState : FSMState
{
    protected override StateID GetID() { return StateID.Chase; }

    private enum FOLLOW_STATE { FAR, MID, CLOSE }
    private FOLLOW_STATE followState = FOLLOW_STATE.FAR;

    private Transform player;
    private Health playerHealth;

    private Enemy enemy;
    private Transform target;

    public float minDistance;
    public float desiredDistance;
    public float maxDistance;
    public float attackDistance;

    private EntityController controller;
    private AiController aiController;

    public float initialAttackDelay;
    public float attackRate;
    public float attackDelay;

    public bool stayAlerted;
    public float visionRadius;

    public float minChangeDirTime, maxChangeDirTime;
    private float currentChangeDirTime;

    public float speed, turnSpeed;

    void Awake()
    {
        controller = GetComponent<EntityController>();
        aiController = GetComponent<AiController>();
        enemy = GetComponent<Enemy>();

        attackDelay = initialAttackDelay;
    }

    void Start()
    {
        player = MyGameManager.instance.GetPlayer().transform;
        playerHealth = player.GetComponentInChildren<Health>();
    }

    public override void Reason(GameObject npc)
    {
        CheckForTarget();

        if (!stayAlerted && target == null)
        {
            enemy.SetTransition(Transition.LostPlayer);
        }
    }

    public override void Act(GameObject npc)
    {
        DebugExtension.DebugCircle(transform.position, Vector3.forward, Color.yellow, visionRadius);

        controller.active = true;
        
        if (attackDelay > 0)
        {
            attackDelay -= Time.fixedDeltaTime;
        }
        if (target != null)
        {
            ChaseTarget();
        }

        controller.RotateTowardsMotion();
    }

    void CheckForTarget()
    {
        if (playerHealth == null || playerHealth.IsDead())
        {
            return;
        }

        AquireTarget();
    }

    void AquireTarget()
    {
        //Transform idealTarget = (target == null) ? player : target;

        //if (target == null)
        //{
        //Vector2 direction = idealTarget.position - transform.position;
        Vector2 direction = player.position - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction.normalized, visionRadius, enemy.aliveTargetLayers);
        target = hit.transform;
        //}
    }

    void ChaseTarget()
    {
        aiController.targetTransform = target;
        aiController.minDistance = minDistance;
        aiController.maxDistance = maxDistance;
        aiController.desiredDistance = desiredDistance;

        controller.speed = speed;
        controller.turnSpeed = turnSpeed;
        controller.RotateTowards(target);

        if (aiController.distanceToTarget < attackDistance)
        {
            Attack();
        }
    }

    void Attack()
    {
        if (attackDelay <= 0)
        {
            attackDelay = attackRate;
            enemy.Attack();
        }
    }
}
