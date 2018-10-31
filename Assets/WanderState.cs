using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderState : FSMState
{
    protected override StateID GetID() { return StateID.Wander; }

    private Transform player;
    private Health playerHealth;

    private Enemy enemy;
    private Transform target;

    private EntityController controller;
    private AiController aiController;

    public float visionRadius;

    public float speed, turnSpeed;
    public float minWanderChangeTime, maxWanderChangeTime;
    private float currentWanderChangeTime;
    
    void Awake()
    {
        controller = GetComponent<EntityController>();
        aiController = GetComponent<AiController>();
        enemy = GetComponent<Enemy>();
    }

    void Start()
    {
        player = MyGameManager.instance.GetPlayer().transform;
        playerHealth = player.GetComponentInChildren<Health>();
    }
    
    public override void DoBeforeEntering()
    {
        base.DoBeforeEntering();
        enemy.alerted = false;
    }

    public override void DoBeforeLeaving()
    {
        base.DoBeforeLeaving();
        enemy.alerted = true;
    }

    public override void Reason(GameObject npc)
    {
        CheckForTarget();
        if (target != null)
        {
            enemy.SetTransition(Transition.SawPlayer);
        }
    }

    void CheckForTarget()
    {
        if (playerHealth == null || playerHealth.IsDead())
        {
            target = null;
        }
        else
        {
            AquireTarget();
        }
    }

    void AquireTarget()
    {
        Vector2 direction = player.position - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction.normalized, visionRadius, enemy.aliveTargetLayers);
        target = hit.transform;
    }

    public override void Act(GameObject npc)
    {
        DebugExtension.DebugCircle(transform.position, Vector3.forward, Color.yellow, visionRadius);

        controller.active = true;

        if (currentWanderChangeTime > 0)
        {
            currentWanderChangeTime -= Time.fixedDeltaTime;
        }
        else
        {
            currentWanderChangeTime = Random.Range(minWanderChangeTime, maxWanderChangeTime);
            controller.SetMoveDirection(Random.insideUnitCircle.normalized);
        }

        aiController.targetTransform = null;

        controller.speed = speed;
        controller.turnSpeed = turnSpeed;
        controller.RotateTowardsMotion();
    }
}
