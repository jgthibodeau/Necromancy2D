using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EntityController))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animator))]

[RequireComponent(typeof(WanderState))]
[RequireComponent(typeof(ChasePlayerState))]
[RequireComponent(typeof(AllyState))]
[RequireComponent(typeof(DeadState))]
public abstract class Enemy : MonoBehaviour
{
    public FSMSystem fsm;
    public StateID initialState;
    [SerializeField]
    private StateID currentState;
    public void SetTransition(Transition t) { fsm.PerformTransition(t); }

    public GameObject aliveGraphics;
    public GameObject deadGraphics;
    public GameObject launchedGraphics;

    public Transform player;
    public SummonCircle summonCircle;
    private EntityController controller;
    private Health health;
    private Outline outline;
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

    public Animator defaultAnimator;
    private Animator aliveAnimator;
    private Animator deadAnimator;
    public Animator currentAnimator;

    public AudioSource audioSource;
    public AudioClip attackClip;
    public float minAttackPitch = 1f;
    public float maxAttackPitch = 1f;

    public GameObject corpseExplosionPrefab;

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
        outline = aliveGraphics.GetComponentInChildren<Outline>();
        rigidBody = GetComponent<Rigidbody2D>();

        audioSource = GetComponent<AudioSource>();
        if (defaultAnimator == null)
        {
            defaultAnimator = GetComponent<Animator>();
        }
        currentAnimator = defaultAnimator;

        aliveAnimator = aliveGraphics.GetComponentInChildren<Animator>();
        deadAnimator = deadGraphics.GetComponentInChildren<Animator>();
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

                SetOutlineColor(0);
                aliveGraphics.SetActive(true);
                deadGraphics.SetActive(false);
                launchedGraphics.SetActive(false);
                
                SetAnimator(aliveAnimator);
                break;
            case StateID.Ally:
                gameObject.layer = allyLayer;
                currentTargetLayers = allyTargetLayers;

                SetOutlineColor(1);
                aliveGraphics.SetActive(true);
                deadGraphics.SetActive(false);
                launchedGraphics.SetActive(false);

                SetAnimator(aliveAnimator);
                break;
            case StateID.Dead:
                gameObject.layer = deadLayer;

                SetOutlineColor(0);
                aliveGraphics.SetActive(false);
                deadGraphics.SetActive(true);
                launchedGraphics.SetActive(false);

                SetAnimator(deadAnimator);

                controller.Stop();
                break;
            case StateID.Launched:
                gameObject.layer = launchedLayer;
                
                SetOutlineColor(1);
                aliveGraphics.SetActive(true);
                deadGraphics.SetActive(false);
                launchedGraphics.SetActive(true);
                
                SetAnimator(aliveAnimator);
                break;
        }
    }

    void SetOutlineColor(int c)
    {
        if (outline != null)
        {
            outline.SetColor(c);
        } else
        {
            outline = aliveGraphics.GetComponentInChildren<Outline>();
        }
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

    public void SetSummonSpot(SummonSpot summonSpot)
    {
        allyState.SetSummonSpot(summonSpot);
    }

    public void SetAllySpeed(float allySpeed)
    {
        Debug.Log("Setting ally speed " + allySpeed);
        allyState.speed = allySpeed;
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

    public virtual void StartAttack() { }

    public virtual void StopAttack() { }
}
