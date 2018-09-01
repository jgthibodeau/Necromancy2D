using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Kill))]
[RequireComponent(typeof(EntityController))]
[RequireComponent(typeof(Capturable))]
public class Missile : MonoBehaviour
{
    private EntityController controller;
    private Rigidbody2D rigidBody;
    private Capturable capturable;
    private Kill kill;
    private VelocityParticles velocityParticles;

    public int damage = 10;

	public LayerMask instantKillLayerMask;

    public LayerMask enemyLayer;
    public LayerMask bulletLayer;
    public LayerMask capturedLayer;
    public bool isEnemy;
    public float visionRadius;
    public Transform player;
    public Transform target;

    public float ignoreEnemyLayerTime;
    public float currentIgnoreEnemyLayerTime;

    void Start() {
        currentIgnoreEnemyLayerTime = ignoreEnemyLayerTime;
        player = GameObject.FindGameObjectWithTag("Player").transform;

        controller = GetComponent<EntityController>();
        rigidBody = GetComponent<Rigidbody2D>();
        capturable = GetComponent<Capturable>();
        kill = GetComponent<Kill>();
        velocityParticles = GetComponentInChildren<VelocityParticles>();
    }

	void Update() {
        if (currentIgnoreEnemyLayerTime > 0)
        {
            currentIgnoreEnemyLayerTime -= Time.deltaTime;
        }

        velocityParticles.play = !capturable.isCaptured;

        if (capturable.isCaptured)
        {
            controller.moveDirection = capturable.gravityWell.transform.position - player.transform.position;

            isEnemy = false;
        }

        if (isEnemy)
        {
            target = player;
        } else
        {
            //target = FindEnemy();
            target = null;
        }

        if (capturable.isCaptured)
        {
            controller.thrust = 0;
        } else
        {
            ChaseTarget();
        }
	}

    Transform FindEnemy()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, visionRadius, enemyLayer);

        Transform tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (Collider2D c in enemies)
        {
            float dist = Vector3.Distance(c.transform.position, currentPos);
            if (dist < minDist)
            {
                tMin = c.transform;
                minDist = dist;
            }
        }
        return tMin;
    }

    void ChaseTarget()
    {
        controller.thrust = 1f;
        if (target != null)
        {
            controller.moveDirection = target.position - transform.position;
        } else
        {
            controller.moveDirection = Vector2.zero;
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (Util.InLayerMask(other.gameObject.layer, bulletLayer) ||
            (Util.InLayerMask(other.gameObject.layer, capturedLayer) && Util.InLayerMask(other.gameObject.layer, capturedLayer)) ||
            (currentIgnoreEnemyLayerTime > 0 && Util.InLayerMask(other.gameObject.layer, enemyLayer)))
        {
            return;
        }

        //Debug.Log("collided with " + other.gameObject.name);

        //destroy if collided with fire
        if (Util.InLayerMask(other.gameObject.layer, instantKillLayerMask))
        {
            GetComponent<Kill>().Die();
        }

        GameObject hit = other.gameObject;
        Health health = hit.GetComponent<Health>();

        if (health != null)
        {
            //Debug.Log("hitting " + health);
            health.Hit(damage, this.gameObject);
        }

        kill.Die();
    }
}
