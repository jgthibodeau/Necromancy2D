using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour {
    public float damage;
    
    void OnCollisionEnter2D(Collision2D other)
    {
        //Debug.Log("collided with " + other.gameObject.name);

        GameObject hit = other.gameObject;
        Health health = hit.GetComponent<Health>();

        if (health != null)
        {
            //Debug.Log("hitting " + health);
            health.Hit(damage, this.gameObject);
        }
    }
}
