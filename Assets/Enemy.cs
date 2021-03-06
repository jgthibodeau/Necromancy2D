﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EntityController))]
[RequireComponent(typeof(AudioSource))]

[RequireComponent(typeof(WanderState))]
[RequireComponent(typeof(ChasePlayerState))]
[RequireComponent(typeof(AllyState))]
[RequireComponent(typeof(DeadState))]
public abstract class Enemy : MonoBehaviour
{
    public bool alerted;

    public FSMSystem fsm;
    public StateID initialState;
    [SerializeField]
    private StateID currentState;
    public void SetTransition(Transition t) { fsm.PerformTransition(t); }

    public GameObject aliveGraphics;
    public GameObject deadGraphics;
    public GameObject launchedGraphics;

    public Collider2D[] defaultColliders;

    public Transform player;
    public SummonCircle summonCircle;
    private EntityController controller;
    private Health health;
    public Outline aliveOutline;
    public Outline deadOutline;
    public Rigidbody2D rigidBody;

    public int enemyLayer;
    public int allyLayer;
    public int deadLayer;
    public int launchedLayer;

    public LayerMask aliveAvoidanceLayers;
    public LayerMask allyAvoidanceLayers;

    public LayerMask aliveTargetLayers;
    public LayerMask allyTargetLayers;

    protected LayerMask currentTargetLayers;
    
    private Animator aliveAnimator;
    private Animator deadAnimator;
    public Animator currentAnimator;

    public AudioSource audioSource;
    public AudioClip attackClip;
    public float minAttackPitch = 1f;
    public float maxAttackPitch = 1f;

    public GameObject corpseExplosionPrefab;

    public Animator attackFlash;
    private Vector3 attackFlashRotation;

    void Awake ()
    {
        RetreiveScripts();

        enemyLayer = LayerMask.NameToLayer("Enemy");
        allyLayer = LayerMask.NameToLayer("Ally");
        deadLayer = LayerMask.NameToLayer("Dead");
        launchedLayer = LayerMask.NameToLayer("Launched");
        MakeFSM();
    }

    public virtual void Start()
    {
        player = MyGameManager.instance.GetPlayer().transform;
    }

    protected WanderState wanderState;
    protected ChasePlayerState chaseState;
    protected AllyState allyState;
    protected DeadState deadState;
    protected LaunchedState launchedState;
    public virtual void MakeFSM()
    {
        if (fsm != null)
        {
            return;
        }

        fsm = new FSMSystem(initialState);

        wanderState = GetComponent<WanderState>();
        wanderState.AddTransition(Transition.LostPlayer, StateID.Wander);
        wanderState.AddTransition(Transition.SawPlayer, StateID.Chase);
        fsm.AddState(wanderState);

        chaseState = GetComponent<ChasePlayerState>();
        chaseState.AddTransition(Transition.SawPlayer, StateID.Chase);
        chaseState.AddTransition(Transition.LostPlayer, StateID.Wander);
        fsm.AddState(chaseState);

        allyState = GetComponent<AllyState>();
        allyState.AddTransition(Transition.Launch, StateID.Launched);
        allyState.AddTransition(Transition.Resurrect, StateID.Ally);
        fsm.AddState(allyState);

        deadState = GetComponent<DeadState>();
        deadState.AddTransition(Transition.Resurrect, StateID.Ally);
        fsm.AddState(deadState);

        launchedState = GetComponent<LaunchedState>();
        launchedState.AddTransition(Transition.Launch, StateID.Launched);
        fsm.AddState(launchedState);

        fsm.AddTransitionToAllStates(Transition.Dead, StateID.Dead);

        Debug.Log("current state " + fsm.CurrentStateID);
    }

    void RetreiveScripts()
    {
        controller = GetComponent<EntityController>();
        health = GetComponent<Health>();
        aliveOutline = aliveGraphics.GetComponentInChildren<Outline>();
        deadOutline = deadGraphics.GetComponentInChildren<Outline>();
        rigidBody = GetComponent<Rigidbody2D>();

        audioSource = GetComponent<AudioSource>();

        aliveAnimator = aliveGraphics.GetComponentInChildren<Animator>();
        deadAnimator = deadGraphics.GetComponentInChildren<Animator>();
        currentAnimator = aliveAnimator;
}

    public bool CanResurrect()
    {
        return fsm.CurrentStateID == StateID.Dead;
    }

    public bool CanExplode()
    {
        return fsm.CurrentStateID == StateID.Dead || fsm.CurrentStateID == StateID.Launched;
    }

    public bool IsAlive()
    {
        return fsm.CurrentStateID == StateID.Wander || fsm.CurrentStateID == StateID.Chase;
    }
    
    void Update()
    {
        //if (lifeState == LIFE_STATE.DEAD)
        if (fsm.CurrentStateID == StateID.Dead)
        {
            StopAttack();
        }
        attackFlash.transform.eulerAngles = attackFlashRotation;

        currentAnimator.SetBool("Ally", currentState == StateID.Ally);
    }

    public virtual void FixedUpdate() {
        currentState = fsm.CurrentStateID;

        if (health.currentHealth <= 0)
        {
            SetTransition(Transition.Dead);
            currentAnimator.SetFloat("Speed", 0);
        }
        else
        {
            fsm.CurrentState.Reason(gameObject);
            currentAnimator.SetFloat("Speed", controller.rb.velocity.magnitude);
        }
        fsm.CurrentState.Act(gameObject);

        
        switch(fsm.CurrentStateID)
        {
            case StateID.Wander:
            case StateID.Chase:
                gameObject.layer = enemyLayer;
                currentTargetLayers = aliveTargetLayers;
                
                aliveGraphics.SetActive(true);
                deadGraphics.SetActive(false);
                launchedGraphics.SetActive(false);
                
                SetAnimator(aliveAnimator);
                break;
            case StateID.Ally:
                gameObject.layer = allyLayer;
                currentTargetLayers = allyTargetLayers;
                
                aliveGraphics.SetActive(true);
                deadGraphics.SetActive(false);
                launchedGraphics.SetActive(false);

                SetAnimator(aliveAnimator);
                break;
            case StateID.Dead:
                gameObject.layer = deadLayer;
                
                aliveGraphics.SetActive(false);
                deadGraphics.SetActive(true);
                launchedGraphics.SetActive(false);

                SetAnimator(deadAnimator);

                controller.Stop();
                break;
            case StateID.Launched:
                gameObject.layer = launchedLayer;
                
                aliveGraphics.SetActive(true);
                deadGraphics.SetActive(false);
                launchedGraphics.SetActive(true);
                
                SetAnimator(aliveAnimator);
                break;
        }
    }

    public void SetAliveOutlineColor(int c)
    {
        if (aliveOutline == null)
        {
            aliveOutline = aliveGraphics.GetComponentInChildren<Outline>();
        }
        aliveOutline.SetColor(c);
    }

    public void SetDeadOutlineColor(int c)
    {
        if (deadOutline == null)
        {
            deadOutline = deadGraphics.GetComponentInChildren<Outline>();
        }
        deadOutline.SetColor(c);
    }

    public void RemoveAliveOutlineColor()
    {
        if (aliveOutline == null)
        {
            aliveOutline = aliveGraphics.GetComponentInChildren<Outline>();
        }
        aliveOutline.RemoveColor();
    }

    public void RemoveDeadOutlineColor()
    {
        if (deadOutline == null)
        {
            deadOutline = deadGraphics.GetComponentInChildren<Outline>();
        }
        deadOutline.RemoveColor();
    }

    void SetAnimator(Animator a)
    {
        if (a != null && currentAnimator != a)
        {
            currentAnimator = a;
        }
    }

    public void Resurrect(SummonSpot summonSpot)
    {
        RetreiveScripts();
        
        SetTransition(Transition.Resurrect);
        health.Resurrect();
        SetSummonSpot(summonSpot);
    }

    public virtual void PreJump() { }
    public virtual void Jump() { }
    public virtual void Land() { }
    public virtual void StopLand() { }

    public void SetSummonSpot(SummonSpot summonSpot)
    {
        allyState.SetSummonSpot(summonSpot);
    }

    public void SetAllySpeed(float allySpeed)
    {
        Debug.Log("Setting ally speed " + allySpeed);
        allyState.speed = allySpeed;
    }

    public void SetAcceleration(float acceleration)
    {
        controller.acceleration = acceleration;
    }

    public void SetAllyAttack(bool attack)
    {
        allyState.attacking = attack;
    }

    public void Launch(Vector2 force)
    {
        SetTransition(Transition.Launch);
        launchedState.SetLaunchForce(force);

        allyState.ResetSummonSpot();

        if (summonCircle != null)
        {
            summonCircle.Remove(this);
        }
    }

    public void Explode()
    {
        GameObject.Instantiate(corpseExplosionPrefab, transform.position, transform.rotation);
        GameObject.Destroy(gameObject);
    }

    public virtual void Attack()
    {
        currentAnimator.SetTrigger("Attack");
        audioSource.pitch = Random.Range(minAttackPitch, maxAttackPitch);
        audioSource.Play();
    }

    public virtual void TelegraphAttack() {
        if (fsm.CurrentStateID != StateID.Ally)
        {
            Debug.Log("Flashing");
            attackFlash.SetTrigger("Flash");
            attackFlashRotation = new Vector3(0, 0, Random.Range(0, 360));
        }
    }

    public virtual void StartAttack() { }

    public virtual void StopAttack() { }
}
