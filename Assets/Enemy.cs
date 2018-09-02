using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EntityController))]
public class Enemy : MonoBehaviour
{
    public Transform player;
    public Health playerHealth;
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

    public Vector2 moveDirection;
    public float thrust;

    void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerHealth = player.GetComponentInChildren<Health>();
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

        Move();
	}

    void CheckForPlayer()
    {
        if (playerHealth.IsDead())
        {
            alerted = false;
            return;
        }

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
        moveDirection = direction;
        if (distanceToPlayer > followDistance)
        {
            currentChangeDirTime = 0;
            thrust = 1f;
        } else
        {
            thrust = 0f;
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
            moveDirection = direction;
            currentChangeDirTime = Random.Range(minChangeDirTime, maxChangeDirTime);

            thrust = Random.Range(0f, 1f);
        }
        else
        {
            currentChangeDirTime -= Time.deltaTime;
        }
    }

    public bool avoidAsteroids = true;
    public LayerMask debrisLayer;
    public float asteroidCheckRange = 6f;
    public float asteroidCheckAngle = 0.15f;
    public float avoidAngle = 10f;
    void Move()
    {
        if (avoidAsteroids)
        {
            Avoidance();
        }

        controller.moveDirection = moveDirection;
        controller.thrust = thrust;
    }

    void Avoidance()
    {
        //if current path leads us into an asteroid, adjust

        Vector2 forward = moveDirection.normalized;
        Vector2 movePerpLeftVector = Vector2.Perpendicular(forward);
        Vector3 left = (forward + movePerpLeftVector * asteroidCheckAngle).normalized;
        Vector3 right = (forward - movePerpLeftVector * asteroidCheckAngle).normalized;
        Debug.DrawRay(transform.position, forward * asteroidCheckRange);
        Debug.DrawRay(transform.position, left * asteroidCheckRange);
        Debug.DrawRay(transform.position, right * asteroidCheckRange);

        bool forwardAsteroid = Physics2D.Raycast(transform.position, forward, asteroidCheckRange, debrisLayer);
        bool leftAsteroid = Physics2D.Raycast(transform.position, left, asteroidCheckRange, debrisLayer);
        bool rightAsteroid = Physics2D.Raycast(transform.position, right, asteroidCheckRange, debrisLayer);
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
                moveDirection = Quaternion.Euler(0, 0, avoidAngle) * moveDirection;
            }
            else
            {
                moveDirection = Quaternion.Euler(0, 0, -avoidAngle) * moveDirection;
            }
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
