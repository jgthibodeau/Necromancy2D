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
    //public enum LIFE_STATE { ALIVE, DEAD, RESURECTED, LAUNCHED }
    //public LIFE_STATE lifeState;

    public FSMSystem fsm;
    public StateID initialState;
    public void SetTransition(Transition t) { fsm.PerformTransition(t); }

    public GameObject aliveGraphics;
    public GameObject deadGraphics;
    public GameObject launchedGraphics;

    public Transform player;
    public SummonCircle summonCircle;

    //public EnemyBehavior enemyBehavior;
    //public AllyBehavior allyBehavior;
    //public LaunchedBehavior launchedBehavior;
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

        //enemyBehavior.enemy = this;
        //allyBehavior.enemy = this;
        //launchedBehavior.enemy = this;
    }

    WanderState wander;
    ChasePlayerState chase;
    AllyState ally;
    DeadState dead;
    LaunchedState launched;
    private void MakeFSM()
    {
        fsm = new FSMSystem(initialState);

        wander = GetComponent<WanderState>();
        wander.AddTransition(Transition.LostPlayer, StateID.Wander);
        wander.AddTransition(Transition.SawPlayer, StateID.Chase);
        wander.AddTransition(Transition.Dead, StateID.Dead);
        fsm.AddState(wander);

        chase = GetComponent<ChasePlayerState>();
        chase.AddTransition(Transition.SawPlayer, StateID.Chase);
        chase.AddTransition(Transition.LostPlayer, StateID.Wander);
        chase.AddTransition(Transition.Dead, StateID.Dead);
        fsm.AddState(chase);

        ally = GetComponent<AllyState>();
        ally.AddTransition(Transition.Launch, StateID.Launched);
        ally.AddTransition(Transition.Dead, StateID.Dead);
        ally.AddTransition(Transition.Resurrect, StateID.Ally);
        fsm.AddState(ally);

        dead = GetComponent<DeadState>();
        dead.AddTransition(Transition.Resurrect, StateID.Ally);
        dead.AddTransition(Transition.Dead, StateID.Dead);
        fsm.AddState(dead);

        launched = GetComponent<LaunchedState>();
        launched.AddTransition(Transition.Launch, StateID.Launched);
        fsm.AddState(launched);

        Debug.Log("current state " + fsm.CurrentStateID);
    }

    void RetreiveScripts()
    {
        //enemyBehavior = GetComponent<EnemyBehavior>();
        //allyBehavior = GetComponent<AllyBehavior>();
        //launchedBehavior = GetComponent<LaunchedBehavior>();
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
        //return lifeState == LIFE_STATE.DEAD;
        return fsm.CurrentStateID == StateID.Dead;
    }

    public bool CanExplode()
    {
        //return lifeState == LIFE_STATE.DEAD || lifeState == LIFE_STATE.LAUNCHED;
        return fsm.CurrentStateID == StateID.Dead || fsm.CurrentStateID == StateID.Launched;
    }

    public bool IsAlive()
    {
        //return lifeState == LIFE_STATE.ALIVE;
        return fsm.CurrentStateID == StateID.Wander || fsm.CurrentStateID == StateID.Chase;
    }

    public virtual void FixedUpdate() {
        if (health.currentHealth <= 0)
        {
            //lifeState = LIFE_STATE.DEAD;
            SetTransition(Transition.Dead);
            currentAnimator.SetFloat("Speed", 0);
        }
        else
        {
            fsm.CurrentState.Reason(gameObject);
            currentAnimator.SetFloat("Speed", controller.rb.velocity.magnitude);
        }
        fsm.CurrentState.Act(gameObject);

        

        //switch(lifeState)
        switch(fsm.CurrentStateID)
        {
            //case LIFE_STATE.ALIVE:
            case StateID.Wander:
            case StateID.Chase:
                gameObject.layer = enemyLayer;
                currentTargetLayers = aliveTargetLayers;

                SetOutlineColor(0);
                aliveGraphics.SetActive(true);
                deadGraphics.SetActive(false);
                launchedGraphics.SetActive(false);
                
                SetAnimator(aliveAnimator);

                //enemyBehavior.DoBehavior();
                break;
            //case LIFE_STATE.RESURECTED:
            case StateID.Ally:
                gameObject.layer = allyLayer;
                currentTargetLayers = allyTargetLayers;

                SetOutlineColor(1);
                aliveGraphics.SetActive(true);
                deadGraphics.SetActive(false);
                launchedGraphics.SetActive(false);

                SetAnimator(aliveAnimator);

                //allyBehavior.DoBehavior();
                break;
            //case LIFE_STATE.DEAD:
            case StateID.Dead:
                gameObject.layer = deadLayer;

                SetOutlineColor(0);
                aliveGraphics.SetActive(false);
                deadGraphics.SetActive(true);
                launchedGraphics.SetActive(false);

                SetAnimator(deadAnimator);

                controller.Stop();
                break;
            //case LIFE_STATE.LAUNCHED:
            case StateID.Launched:
                gameObject.layer = launchedLayer;
                
                SetOutlineColor(1);
                aliveGraphics.SetActive(true);
                deadGraphics.SetActive(false);
                launchedGraphics.SetActive(true);
                
                SetAnimator(aliveAnimator);

                //launchedBehavior.DoBehavior();
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

        //lifeState = LIFE_STATE.RESURECTED;
        SetTransition(Transition.Resurrect);
        health.Resurrect();
        SetSummonSpot(summonSpot);
    }

    public void SetSummonSpot(SummonSpot summonSpot)
    {
        ally.SetSummonSpot(summonSpot);
        //    if (allyBehavior.summonSpot != null)
        //    {
        //        allyBehavior.summonSpot.SetEnemy(null);
        //    }
        //    allyBehavior.summonSpot = summonSpot;
    }

    public void SetAllySpeed(float allySpeed)
    {
        Debug.Log("Setting ally speed " + allySpeed);
        //allyBehavior.speed = allySpeed;
        ally.speed = allySpeed;
    }

    public void SetAllyAttack(bool attack)
    {
        ally.attacking = attack;
    }

    public void Launch(Vector2 force)
    {
        //lifeState = LIFE_STATE.LAUNCHED;
        SetTransition(Transition.Launch);
        //launchedBehavior.launchForce = force;
        launched.SetLaunchForce(force);

        //if (allyBehavior.summonSpot != null)
        //{
        //    allyBehavior.summonSpot.enemy = null;
        //    allyBehavior.summonSpot = null;
        //}
        ally.ResetSummonSpot();

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

    public float avoidanceAngle, avoidanceRange;

    //public Vector2 Avoidance(Vector2 direction)
    //{
    //    LayerMask avoidLayers = lifeState == LIFE_STATE.ALIVE ? aliveAvoidanceLayers : allyAvoidanceLayers;

    //    Vector2 forward = direction.normalized;
    //    Vector2 movePerpLeftVector = Vector2.Perpendicular(forward);
    //    Vector3 left = (forward + movePerpLeftVector * avoidanceAngle).normalized;
    //    Vector3 right = (forward - movePerpLeftVector * avoidanceAngle).normalized;
    //    Debug.DrawRay(transform.position, forward * avoidanceRange);
    //    Debug.DrawRay(transform.position, left * avoidanceRange);
    //    Debug.DrawRay(transform.position, right * avoidanceRange);

    //    bool forwardAsteroid = Physics2D.Raycast(transform.position, forward, avoidanceRange, avoidLayers);
    //    bool leftAsteroid = Physics2D.Raycast(transform.position, left, avoidanceRange, avoidLayers);
    //    bool rightAsteroid = Physics2D.Raycast(transform.position, right, avoidanceRange, avoidLayers);
    //    bool anyAsteroids = forwardAsteroid || leftAsteroid || rightAsteroid;

    //    if (anyAsteroids)
    //    {
    //        bool avoidToLeft = false;
    //        if (leftAsteroid == rightAsteroid)
    //        {
    //            avoidToLeft = (Random.value > 0.5f);
    //        }
    //        else if (rightAsteroid)
    //        {
    //            avoidToLeft = true;
    //        }

    //        if (avoidToLeft)
    //        {
    //            direction = Quaternion.Euler(0, 0, avoidanceAngle) * direction;
    //        }
    //        else
    //        {
    //            direction = Quaternion.Euler(0, 0, -avoidanceAngle) * direction;
    //        }
    //    }

    //    return direction;
    //}
}
