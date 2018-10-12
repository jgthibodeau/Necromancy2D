using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Kill))]
public class Debris : MonoBehaviour {
    Rigidbody2D rb;
    public bool collide = true;
    public Vector2 initialVelocityRange;
    public LayerMask capturedLayer;

    public float minCollisionSpeedToDamage;
    public float damage;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();

        rb.velocity = new Vector2(Random.Range(-initialVelocityRange.x, initialVelocityRange.x), Random.Range(-initialVelocityRange.y, initialVelocityRange.y));
	}

    void OnCollisionEnter2D(Collision2D other)
    {
        if (!collide || (Util.InLayerMask(other.gameObject.layer, capturedLayer) && Util.InLayerMask(other.gameObject.layer, capturedLayer)) ||
            other.relativeVelocity.magnitude < minCollisionSpeedToDamage)
        {
            return;
        }

        //Debug.Log("collided with " + other.gameObject.name);

        GameObject hit = other.gameObject;
        Health health = hit.GetComponent<Health>();

        if (health != null)
        {
            //Debug.Log("hitting " + health);
            health.Hit(damage, this.gameObject);
            GetComponent<Kill>().Die();
        }
    }
}
