using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Kill))]
public class Bullet : MonoBehaviour {
    private Rigidbody2D rb;
    public float bulletForce;
    public float bulletDamage;
    public LayerMask enemyLayer;
    public LayerMask bulletLayer;
    public LayerMask capturedLayer;
    public float ignoreEnemyLayerTime;
    public float currentIgnoreEnemyLayerTime;

    public bool rotateToVelocity;

    void Start()
    {
        currentIgnoreEnemyLayerTime = ignoreEnemyLayerTime;
        rb = GetComponent<Rigidbody2D>();
        Vector2 newVelocity = transform.up * bulletForce;
        newVelocity += rb.velocity;
        rb.velocity = newVelocity;
    }

    void Update()
    {
        if (currentIgnoreEnemyLayerTime > 0)
        {
            currentIgnoreEnemyLayerTime -= Time.deltaTime;
        }

        if (rotateToVelocity)
        {
            rb.rotation = Vector2.Angle(Vector2.up, rb.velocity);
        }
    }

	// Update is called once per frame
	void FixedUpdate ()
    {
        //rb.velocity = transform.up * bulletForce;
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

        GameObject hit = other.gameObject;
        Health health = hit.GetComponent<Health>();

        if (health != null)
        {
            //Debug.Log("hitting " + health);
            health.Hit(bulletDamage, this.gameObject);
        }

        GetComponent<Kill>().Die();
    }
}
