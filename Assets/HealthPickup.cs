using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Kill))]
public class HealthPickup : MonoBehaviour
{
    public float amount;

    void Start()
    {
        Debris debris = GetComponent<Debris>();
        if (debris != null)
        {
            debris.collide = false;
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag != "Player")
        {
            return;
        }

        //Debug.Log("collided with " + other.gameObject.name);

        GameObject hit = other.gameObject;
        Health health = hit.GetComponent<Health>();

        if (health.IsDead())
        {
            return;
        }

        if (health != null)
        {
            //Debug.Log("hitting " + health);
            health.Heal(amount);
            GetComponent<Kill>().Die();
        }
    }
}
