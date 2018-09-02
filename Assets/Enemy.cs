using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EntityController))]
public class Enemy : MonoBehaviour
{
    public Transform player;
    public float distanceToPlayer;
    public float followDistance;

    private EntityController controller;
    public GameObject bullet;
	public Transform firePosition;
    public float fireRandomness;

    public float fireRate;
    private float fireDelay;

	private Rigidbody2D rigidBody;

    public float visionRadius, alertVisionRadius;
    public float fireRange, fireLead;
    public bool alerted;

    public float minChangeDirTime, maxChangeDirTime;
    public float currentChangeDirTime;

    public float wanderSpeed, chaseSpeed;
    public float wanderTurnSpeed, chaseTurnSpeed;

    void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        controller = GetComponent<EntityController>();
        rigidBody = GetComponent<Rigidbody2D>();
    }

	void Update () {
        if (fireDelay > 0)
        {
            fireDelay -= Time.deltaTime;
        }

        CheckForPlayer();
        DebugExtension.DebugCircle(transform.position, Vector3.forward, Color.blue, fireRange);
        if (alerted)
        {
            DebugExtension.DebugCircle(transform.position, Vector3.forward, Color.red, alertVisionRadius);
            DebugExtension.DebugCircle(transform.position, Vector3.forward, Color.red, visionRadius);
            controller.speed = chaseSpeed;
            controller.normalTurnSpeed = chaseTurnSpeed;
            controller.thrustTurnSpeed = chaseTurnSpeed;
            ChasePlayer();
        } else
        {
            DebugExtension.DebugCircle(transform.position, Vector3.forward, Color.red, alertVisionRadius);
            DebugExtension.DebugCircle(transform.position, Vector3.forward, Color.yellow, visionRadius);
            controller.speed = wanderSpeed;
            controller.normalTurnSpeed = wanderTurnSpeed;
            controller.thrustTurnSpeed = wanderTurnSpeed;
            Wander();
        }
	}

    void CheckForPlayer()
    {
        distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
        if (alerted && distanceToPlayer > alertVisionRadius)
        {
            alerted = false;
        } else if (!alerted && distanceToPlayer < visionRadius)
        {
            alerted = true;
        }
    }

    void ChasePlayer()
    {
        Vector3 targetDir = player.transform.position;
        Vector3 playerVelocity = player.GetComponent<Rigidbody2D>().velocity;
        float perpendicularPlayerVelocity = Vector3.Dot(playerVelocity, transform.right);

        targetDir += perpendicularPlayerVelocity * transform.right * fireLead;
        Vector2 direction = targetDir - transform.position;
        controller.moveDirection = direction;
        if (distanceToPlayer > followDistance)
        {
            currentChangeDirTime = 0;
            controller.thrust = 1f;
        } else
        {
            controller.thrust = 0f;
        }
        if (distanceToPlayer < fireRange)
        {
            AttackPlayer();
        }
    }

    void AttackPlayer()
    {
        if (fireDelay <= 0)
        {
            Fire();
        }
    }

    void Wander()
    {
        //move randomly
        if (currentChangeDirTime <= 0)
        {
            Vector2 direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            controller.moveDirection = direction;
            currentChangeDirTime = Random.Range(minChangeDirTime, maxChangeDirTime);

            controller.thrust = Random.Range(0f, 1f);
        }
        else
        {
            currentChangeDirTime -= Time.deltaTime;
        }
    }

	void Fire() {
        fireDelay = fireRate;

        Vector3 position = firePosition.position;
        position.x += Random.Range(-fireRandomness, fireRandomness);
        position.y += Random.Range(-fireRandomness, fireRandomness);
        GameObject newBullet = GameObject.Instantiate (bullet, position, transform.rotation);
        newBullet.GetComponent<Rigidbody2D>().rotation = rigidBody.rotation;
        newBullet.GetComponent<Rigidbody2D>().velocity = rigidBody.velocity;
        //newBullet.GetComponent<Rigidbody2D> ().AddForce (newBullet.transform.up * bulletForce, ForceMode2D.Impulse);
    }
}
