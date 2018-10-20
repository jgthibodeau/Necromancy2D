using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Kill))]
public class Bullet : MonoBehaviour {
    private Rigidbody2D rb;
    public float bulletForce;
    public float bulletDamage;
    public LayerMask bulletLayer;

    public bool rotateToVelocity;

    public Vector2 velocity;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        velocity = transform.forward * bulletForce;
        //velocity += rb.velocity;
        rb.velocity = velocity;
    }

	// Update is called once per frame
	void FixedUpdate ()
    {
        //rb.velocity = transform.up * bulletForce;

        rb.velocity = velocity;
        if (rotateToVelocity)
        {
            rb.rotation = Vector2.SignedAngle(Vector2.up, rb.velocity);
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
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
