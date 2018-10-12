using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof(EntityController))]
[RequireComponent(typeof(Health))]
public class Player : MonoBehaviour
{
    private LevelManager levelManager;
    private EntityController controller;
    private EnemyLauncher enemyLauncher;
    private Health health;
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
        screenShake = ScreenShake.instance;
    }

    void Update()
    {

        if (!health.IsDead())
        {
            Aim();
            if (!aiming)
            {
                Attack();
            }
            MovePlayer();
        } else
        {
            controller.Stop();
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
        }
        if (!aiming || !Util.GetButton("Launch"))
        {
            launching = false;
            enemyLauncher.LaunchEnemy();
        }
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

        if (Util.GetButton("Attack") && attackDelay <= 0 && summonCircle.HasSummons())
        {
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
            Vector2 moveDirection = new Vector2(Util.GetAxis("Horizontal"), Util.GetAxis("Vertical"));
            controller.SetMoveDirection(moveDirection);

            //if mouse outside of summon area that is populated with minions
            if (!summonCircle.IsMouseInSummonSpots() && !aiming)
            {
                controller.RotateTowards(summonCircle.transform);
            }
        }
        else
        {
            controller.SetMoveDirection(Vector2.zero);
        }
    }

    void KillPlayer()
    {
        controller.Stop();
        levelManager.OnPlayerDeath();
    }

    public void TakeDamage(float damage)
    {
        screenShake.Shake(playerDamageShakeTime, damage * playerDamageShakeScale);
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
