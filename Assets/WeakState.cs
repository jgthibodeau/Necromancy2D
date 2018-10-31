using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakState : FSMState
{
    protected override StateID GetID() { return StateID.Weak; }

    private Enemy enemy;
    private EntityController controller;
    private WeakSpotHealth weakSpotHealth;

    public enum WEAK_STATE { GOING_DOWN, DOWN, GETTING_UP, UP }
    public WEAK_STATE currentWeakState;

    public float goDownDelay, getUpDelay, maxWeakTime;
    private float currentTimer;

    public void Start()
    {
        enemy = GetComponent<Enemy>();
        controller = GetComponent<EntityController>();
        weakSpotHealth = GetComponent<WeakSpotHealth>();
    }
    
    public override void DoBeforeEntering()
    {
        base.DoBeforeEntering();
        weakSpotHealth.DisableNormalSpots();

        currentWeakState = WEAK_STATE.GOING_DOWN;
        currentTimer = goDownDelay;

        enemy.currentAnimator.SetTrigger("Knock");
    }

    public override void DoBeforeLeaving()
    {
        base.DoBeforeLeaving();
        weakSpotHealth.EnableNormalSpots();
    }

    public override void Reason(GameObject npc)
    {
        if (currentWeakState == WEAK_STATE.UP)
        {
            enemy.SetTransition(Transition.CooldownFinished);
        }
    }

    public override void Act(GameObject npc)
    {
        controller.Stop();

        switch(currentWeakState)
        {
            case WEAK_STATE.GOING_DOWN:
                if (currentTimer > 0)
                {
                    currentTimer -= Time.fixedDeltaTime;
                }
                else
                {
                    weakSpotHealth.EnableWeakSpot();
                    currentWeakState = WEAK_STATE.DOWN;
                    currentTimer = maxWeakTime;
                }
                break;
            case WEAK_STATE.DOWN:
                if (currentTimer > 0)
                {
                    currentTimer -= Time.fixedDeltaTime;
                }
                if (currentTimer <= 0 || !weakSpotHealth.IsWeak())
                {
                    weakSpotHealth.DisableWeakSpot();
                    currentWeakState = WEAK_STATE.GETTING_UP;
                    currentTimer = getUpDelay;
                    enemy.currentAnimator.SetTrigger("Get Up");
                }
                break;
            case WEAK_STATE.GETTING_UP:
                if (currentTimer > 0)
                {
                    currentTimer -= Time.fixedDeltaTime;
                }
                else
                {
                    currentWeakState = WEAK_STATE.UP;
                }
                break;
        }
    }
}
