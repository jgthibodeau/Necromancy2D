using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossJumpState : FSMState
{
    protected override StateID GetID() { return StateID.BossJump; }

    private Enemy enemy;
    private AiController aiController;
    private EntityController controller;
    private Transform player;

    public float airTime;
    public float landDelayTime;
    [SerializeField]
    private float currentTime;

    public bool crossHairInAir, crossHairInDelay;

    private enum JumpState { PREPARE, JUMPING, IN_AIR_MOVING, IN_AIR_FROZEN, FALLING, LANDING, DONE }
    [SerializeField]
    private JumpState jumpState;

    public float minDistance;
    public float desiredDistance;
    public float maxDistance;

    public bool exactFollowPlayer = true;
    public float speed;
    public float turnSpeed;

    public GameObject shockwave;

    public GameObject crosshair;
    public Projector jumpProjector;
    public float minProjectorSize, maxProjectorSize;

    void Start()
    {
        enemy = GetComponent<Enemy>();
        aiController = GetComponent<AiController>();
        controller = GetComponent<EntityController>();
        player = MyGameManager.instance.GetPlayer().transform;
    }

    public override void DoBeforeEntering()
    {
        base.DoBeforeEntering();
        jumpState = JumpState.PREPARE;
    }

    public override void Reason(GameObject npc)
    {
        if (jumpState == JumpState.DONE)
        {
            enemy.SetTransition(Transition.SawPlayer);
        }
    }

    public override void Act(GameObject npc)
    {

        switch (jumpState)
        {
            case JumpState.PREPARE:
                controller.Stop();
                enemy.currentAnimator.SetTrigger("Jump");
                jumpState = JumpState.JUMPING;
                currentTime = airTime;
                break;
            case JumpState.IN_AIR_MOVING:
                //show reticle for landing zone
                if (crossHairInAir && !crosshair.activeSelf) {
                    crosshair.SetActive(true);
                }

                if (currentTime > 0)
                {
                    currentTime -= Time.fixedDeltaTime;

                    if (exactFollowPlayer)
                    {
                        transform.position = player.position;
                    }
                    else
                    {
                        aiController.targetTransform = player;

                        aiController.minDistance = minDistance;
                        aiController.maxDistance = maxDistance;
                        aiController.desiredDistance = desiredDistance;

                        controller.speed = speed;
                        controller.turnSpeed = turnSpeed;
                        controller.RotateTowards(player);
                    }
                }
                else
                {
                    currentTime = landDelayTime;
                    jumpState = JumpState.IN_AIR_FROZEN;
                    controller.Stop();
                }
                break;
            case JumpState.IN_AIR_FROZEN:
                //show reticle for landing zone
                if (crossHairInDelay && !crosshair.activeSelf)
                {
                    crosshair.SetActive(true);
                }

                UpdateProjector(currentTime, landDelayTime);

                if (currentTime > 0)
                {
                    currentTime -= Time.fixedDeltaTime;

                    controller.Stop();
                    controller.turnSpeed = turnSpeed;
                    controller.RotateTowards(player);
                }
                else
                {
                    controller.Stop();
                    enemy.currentAnimator.SetTrigger("Land");
                    jumpState = JumpState.FALLING;
                }
                break;
        }
    }

    private void UpdateProjector(float currentTime, float maxTime)
    {
        float size = maxProjectorSize - Util.ConvertScale(0, maxTime, minProjectorSize, maxProjectorSize, currentTime);
        jumpProjector.orthographicSize = size;
    }

    public void PreJump()
    {
        foreach (Collider2D c in enemy.defaultColliders)
        {
            c.enabled = false;
        }
    }

    public void Jump()
    {
        transform.position = player.position;
        jumpState = JumpState.IN_AIR_MOVING;
    }

    public void Land()
    {
        Debug.Log("landing");
        GameObject.Instantiate(shockwave, transform.position, Quaternion.identity);

        jumpState = JumpState.LANDING;
        crosshair.SetActive(false);

        foreach (Collider2D c in enemy.defaultColliders)
        {
            c.enabled = true;
        }
    }

    public void StopLand()
    {
        Debug.Log("done");
        //done
        jumpState = JumpState.DONE;
    }
}
