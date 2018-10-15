using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EntityController))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animator))]
public abstract class Enemy : MonoBehaviour
{
    public enum LIFE_STATE { ALIVE, DEAD, RESURECTED, LAUNCHED }
    public LIFE_STATE lifeState;
    
    public GameObject aliveGraphics;
    public GameObject resurectedGraphics;
    public GameObject deadGraphics;
    public GameObject launchedGraphics;

    public Transform player;
    public SummonCircle summonCircle;

    public EnemyBehavior enemyBehavior;
    public AllyBehavior allyBehavior;
    public LaunchedBehavior launchedBehavior;
    private EntityController controller;
    private Health health;

    public int enemyLayer;
    public int allyLayer;
    public int deadLayer;
    public int launchedLayer;

    public LayerMask aliveAvoidanceLayers;
    public LayerMask allyAvoidanceLayers;

    public LayerMask aliveTargetLayers;
    public LayerMask allyTargetLayers;

    protected LayerMask currentTargetLayers;

    public Animator animator;
    public AudioSource audioSource;
    public AudioClip attackClip;

    public GameObject corpseExplosionPrefab;

    void Awake ()
    {
        RetreiveScripts();

        enemyLayer = LayerMask.NameToLayer("Enemy");
        allyLayer = LayerMask.NameToLayer("Ally");
        deadLayer = LayerMask.NameToLayer("Dead");
        launchedLayer = LayerMask.NameToLayer("Launched");
    }

    void Start()
    {
        player = MyGameManager.instance.GetPlayer().transform;

        enemyBehavior.enemy = this;
        allyBehavior.enemy = this;
        launchedBehavior.enemy = this;
    }

    void RetreiveScripts()
    {
        enemyBehavior = GetComponent<EnemyBehavior>();
        allyBehavior = GetComponent<AllyBehavior>();
        launchedBehavior = GetComponent<LaunchedBehavior>();
        controller = GetComponent<EntityController>();
        health = GetComponent<Health>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    public bool CanResurrect()
    {
        return lifeState == LIFE_STATE.DEAD;
    }

    public bool CanExplode()
    {
        return lifeState == LIFE_STATE.DEAD || lifeState == LIFE_STATE.LAUNCHED;
    }

    public virtual void FixedUpdate() {
        if (health.currentHealth <= 0)
        {
            lifeState = LIFE_STATE.DEAD;
        }

        switch(lifeState)
        {
            case LIFE_STATE.ALIVE:
                gameObject.layer = enemyLayer;
                currentTargetLayers = aliveTargetLayers;

                aliveGraphics.SetActive(true);
                resurectedGraphics.SetActive(false);
                deadGraphics.SetActive(false);
                launchedGraphics.SetActive(false);

                enemyBehavior.DoBehavior();
                break;
            case LIFE_STATE.RESURECTED:
                gameObject.layer = allyLayer;
                currentTargetLayers = allyTargetLayers;

                aliveGraphics.SetActive(false);
                resurectedGraphics.SetActive(true);
                deadGraphics.SetActive(false);
                launchedGraphics.SetActive(false);

                allyBehavior.DoBehavior();
                break;
            case LIFE_STATE.DEAD:
                gameObject.layer = deadLayer;

                aliveGraphics.SetActive(false);
                resurectedGraphics.SetActive(false);
                deadGraphics.SetActive(true);
                launchedGraphics.SetActive(false);

                controller.Stop();
                break;
            case LIFE_STATE.LAUNCHED:
                gameObject.layer = launchedLayer;

                aliveGraphics.SetActive(false);
                resurectedGraphics.SetActive(false);
                deadGraphics.SetActive(false);
                launchedGraphics.SetActive(true);

                launchedBehavior.DoBehavior();
                break;
        }
    }

    public void Resurrect(SummonSpot summonSpot)
    {
        RetreiveScripts();

        lifeState = LIFE_STATE.RESURECTED;
        health.currentHealth = health.resurrectHealth;
        health.maxHealth = health.resurrectHealth;
        SetSummonSpot(summonSpot);
    }

    public void SetSummonSpot(SummonSpot summonSpot)
    {
        if (allyBehavior.summonSpot != null)
        {
            allyBehavior.summonSpot.SetEnemy(null);
        }
        allyBehavior.summonSpot = summonSpot;
    }

    public void Launch(Vector2 force)
    {
        lifeState = LIFE_STATE.LAUNCHED;
        launchedBehavior.launched = true;
        launchedBehavior.launchForce = force;

        if (allyBehavior.summonSpot != null)
        {
            allyBehavior.summonSpot.enemy = null;
            allyBehavior.summonSpot = null;
        }

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
        animator.SetTrigger("Attack");
        audioSource.Play();
    }

    public virtual void StartAttack() { }

    public virtual void StopAttack() { }

    public float avoidanceAngle, avoidanceRange;

    public Vector2 Avoidance(Vector2 direction)
    {
        LayerMask avoidLayers = lifeState == LIFE_STATE.ALIVE ? aliveAvoidanceLayers : allyAvoidanceLayers;

        Vector2 forward = direction.normalized;
        Vector2 movePerpLeftVector = Vector2.Perpendicular(forward);
        Vector3 left = (forward + movePerpLeftVector * avoidanceAngle).normalized;
        Vector3 right = (forward - movePerpLeftVector * avoidanceAngle).normalized;
        Debug.DrawRay(transform.position, forward * avoidanceRange);
        Debug.DrawRay(transform.position, left * avoidanceRange);
        Debug.DrawRay(transform.position, right * avoidanceRange);

        bool forwardAsteroid = Physics2D.Raycast(transform.position, forward, avoidanceRange, avoidLayers);
        bool leftAsteroid = Physics2D.Raycast(transform.position, left, avoidanceRange, avoidLayers);
        bool rightAsteroid = Physics2D.Raycast(transform.position, right, avoidanceRange, avoidLayers);
        bool anyAsteroids = forwardAsteroid || leftAsteroid || rightAsteroid;

        if (anyAsteroids)
        {
            bool avoidToLeft = false;
            if (leftAsteroid == rightAsteroid)
            {
                avoidToLeft = (Random.value > 0.5f);
            }
            else if (rightAsteroid)
            {
                avoidToLeft = true;
            }

            if (avoidToLeft)
            {
                direction = Quaternion.Euler(0, 0, avoidanceAngle) * direction;
            }
            else
            {
                direction = Quaternion.Euler(0, 0, -avoidanceAngle) * direction;
            }
        }

        return direction;
    }
}
