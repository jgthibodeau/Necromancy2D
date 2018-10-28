using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof(EntityController))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Stamina))]
[RequireComponent(typeof(AudioSource))]
public class Player : MonoBehaviour
{
    private LevelManager levelManager;
    private EntityController controller;
    private EnemyLauncher enemyLauncher;
    private Health health;
    private Stamina stamina;
    private Rigidbody2D rigidbody;
    private Animator animator;
    private AudioSource audioSource;
    public AudioClip dashClip;
    public float minDashPitch = 1f;
    public float maxDashPitch = 1f;

    [Range(0f, 1f)]
    public float criticalHealthPercent = 0.25f;

    public float attackRate;
    private float attackDelay;

    public float playerDamageShakeTime = 0.25f;
    public float playerDamageShakeScale = 0.1f;

    public float attackShakeTime = 0.25f;
    public float attackShakeScale = 0.1f;

    ScreenShake screenShake;

    public SummonCircle summonCircle;

	void Start()
    {
        levelManager = LevelManager.instance;
        controller = GetComponent<EntityController>();
        enemyLauncher = GetComponent<EnemyLauncher>();
        health = GetComponent<Health>();
        stamina = GetComponent<Stamina>();
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
        screenShake = ScreenShake.instance;
    }

    public Animator GetAnimator()
    {
        return animator;
    }

    void Update()
    {

        if (!health.IsDead())
        {
            animator.SetFloat("Speed", rigidbody.velocity.magnitude);

            Aim();
            //if (!aiming)
            if (!enemyLauncher.HasEnemy())
            {
                Attack();
            }
            MovePlayer();
        } else if (!alreadyDead)
        {
            KillPlayer();
        }
    }

    private bool aiming;
    private bool launching;
    void Aim()
    {
        aiming = Util.GetButton("Aim");
        if (aiming)
        {
            if (Util.GetButtonDown("Launch") && !launching)
            {
                launching = true;
                enemyLauncher.SetEnemyToLaunch();
            }

            if (!Util.GetButton("Launch"))
            {
                launching = false;
                enemyLauncher.LaunchEnemy();
            }

            animator.SetBool("Aim", enemyLauncher.HasEnemy());
        } else
        {
            enemyLauncher.Clear();
        }
        
    }

    private bool dashing;
    private bool secondDash;
    public float dashTime;
    public float dashRechargeTime;
    public float dashSpeed;
    public float normalSpeed;
    private float remainingDashTime;
    private float remainingDashRechargeTime;
    public ParticleSystem dashEffect;
    public GameObject dashBar;
    public Image dashBarFill;
    void Dash()
    {
        if (!dashing)
        {
            if (stamina.HasStamina() && (Util.GetButtonDown("Dash") || secondDash))
            {
                audioSource.pitch = Random.Range(minDashPitch, maxDashPitch);
                audioSource.PlayOneShot(dashClip);
                stamina.UseStamina();
                dashEffect.Play();
                dashing = true;
                secondDash = false;
                remainingDashTime = dashTime;
                controller.speed = dashSpeed;
                controller.NormalizeMoveDirection();
            }
            else
            {
                dashEffect.Stop();
            }
        }
        else
        {
            //TODO if pressed dash again, queue up a second dash
            if (Util.GetButtonDown("Dash"))
            {
                secondDash = true;
            }

            if (remainingDashTime > 0)
            {
                remainingDashTime -= Time.deltaTime;
            }
            else
            {
                dashing = false;
                controller.speed = normalSpeed;
            }
        }

        //if (!dashing)
        //{
        //    if (remainingDashRechargeTime > 0)
        //    {
        //        dashBar.SetActive(true);
        //        dashBarFill.fillAmount = 1f - remainingDashRechargeTime / dashRechargeTime;
        //        dashEffect.Stop();
        //        remainingDashRechargeTime -= Time.deltaTime;
        //    } else  if (Util.GetButtonDown("Dash"))
        //    {
        //        dashBar.SetActive(false);
        //        dashEffect.Play();
        //        dashing = true;
        //        remainingDashTime = dashTime;
        //        controller.speed = dashSpeed;
        //        controller.NormalizeMoveDirection();
        //    } else
        //    {
        //        dashBar.SetActive(false);
        //    }
        //} else
        //{
        //    dashBar.SetActive(true);
        //    dashBarFill.fillAmount = 0f;

        //    if (remainingDashTime > 0)
        //    {
        //        remainingDashTime -= Time.deltaTime;
        //    } else
        //    {
        //        dashing = false;
        //        remainingDashRechargeTime = dashRechargeTime;
        //        controller.speed = normalSpeed;
        //    }
        //}
    }

    private bool attacking;
    public float attackTime;
    void Attack()
    {
        if (attackDelay > 0)
        {
            attackDelay -= Time.deltaTime;
        }

        //if (Util.GetButton("Attack") && attackDelay <= 0 && summonCircle.HasSummons())
        //{
        //    attackDelay = attackRate;
        //    //screenShake.Shake(attackShakeTime, attackShakeScale);

        //    foreach (Enemy enemy in summonCircle.summons)
        //    {
        //        enemy.Attack();
        //    }
        //}

        if (Util.GetButton("Attack") && !aiming && attackDelay <= 0 && summonCircle.HasSummons())
        {
            animator.SetTrigger("Attack");
            attackDelay = attackRate;
            screenShake.Shake(attackShakeTime, attackShakeScale);
            Swarm();
        }
    }

    void Swarm()
    {
        if (attacking)
        {
            return;
        }
        animator.SetTrigger("Attack");

        foreach (Enemy enemy in summonCircle.summons)
        {
            enemy.Attack();
        }

        attacking = true;
        summonCircle.Attack();
        StartCoroutine(WaitForAttack());
    }

    IEnumerator WaitForAttack()
    {
        yield return new WaitForSeconds(attackTime);
        summonCircle.StopAttack();
        attacking = false ;
    }

    void MovePlayer()
    {
        if (!attacking)
        {
            Dash();
            if (!dashing)
            {
                Vector2 moveDirection = new Vector2(Util.GetAxis("Horizontal"), Util.GetAxis("Vertical"));
                controller.SetMoveDirection(moveDirection);
                controller.speed = normalSpeed;
            }

            ////if mouse outside of summon area that is populated with minions
            //if (!summonCircle.IsMouseInSummonSpots() && !aiming)
            //{
            //    controller.RotateTowards(summonCircle.transform);
            //}
        }
        //else
        //{
        //    controller.SetMoveDirection(Vector2.zero);
        //}
        //if mouse outside of summon area that is populated with minions
        if (!summonCircle.IsMouseInSummonSpots() && !aiming)
        {
            controller.RotateTowards(summonCircle.transform);
        }
    }

    bool alreadyDead = false;
    void KillPlayer()
    {
        alreadyDead = true;
        controller.Stop();
        levelManager.OnPlayerDeath();
        animator.SetBool("Dead", true);
    }

    public void TakeDamage(float damage)
    {
        screenShake.Shake(playerDamageShakeTime, damage * playerDamageShakeScale);
        if (!alreadyDead)
        {
            animator.SetTrigger("Damage");
        }
    }

    public bool IsDead()
    {
        return health.IsDead();
    }

    public bool IsCriticalHealth()
    {
        return health.Percentage() < criticalHealthPercent;
    }
}
