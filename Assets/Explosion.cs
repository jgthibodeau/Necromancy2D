using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Explosion : MonoBehaviour
{
	public float radius = 1f;
	public float forceScale = 1f;
	public ForceMode2D forceMode = ForceMode2D.Impulse;
	public float damage = 1f;
	public bool scaleForceByDistance = true;
	public bool scaleDamageByDistance = true;
	public bool splashEffects = true;
	public bool splashDamage = true;

    public LayerMask layermask;

    public List<Health> healths = new List<Health> ();

	void Start(){
		Collider2D[] colliders = Physics2D.OverlapCircleAll (transform.position, radius, layermask);
		HashSet<Rigidbody2D> rigidbodies = new HashSet<Rigidbody2D>();
		foreach (Collider2D collider in colliders) {
			float explosionPercent;

            //for each unique rigidbody
            Rigidbody2D rigidBody = collider.GetComponentInParent<Rigidbody2D> ();
			if (rigidBody != null && !rigidbodies.Contains (rigidBody)) {
				rigidbodies.Add (rigidBody);
				//add force based on distance and away from explosion point
				//					Vector3 center = rigidBody.position;
				Vector3 center = rigidBody.transform.TransformPoint (rigidBody.centerOfMass);
				Vector3 direction = center - transform.position;
				if (scaleForceByDistance) {
					explosionPercent = 1 - (Vector3.Magnitude (direction) / radius);
				} else {
					explosionPercent = 1f;
				}
				if (explosionPercent >= 0) {
					Vector3 explosionForce = direction.normalized * forceScale;
					if (scaleForceByDistance) {
						explosionForce *= explosionPercent;
					}
//					Debug.Log ("adding force to rigidbody " + rigidBody + " " + explosionForce);
					rigidBody.AddForce (explosionForce, forceMode);
				}
			}

			if (splashEffects)
            {
                Enemy enemy = collider.gameObject.GetComponent<Enemy>();
                if (enemy != null && enemy.CanExplode())
                {
                    enemy.Explode();
                }
                else
                {
                    Health health = collider.gameObject.GetComponent<Health>();
                    if (health != null && !healths.Contains(health))
                    {
                        healths.Add(health);

                        float thisDamage = 0;
                        if (splashDamage)
                        {
                            if (scaleDamageByDistance)
                            {
                                Vector3 center = collider.transform.position;
                                Vector3 direction = center - transform.position;
                                explosionPercent = 1 - (Vector3.Magnitude(direction) / radius);
                            }
                            else
                            {
                                explosionPercent = 1;
                            }
                            thisDamage = damage * explosionPercent;
                        }

                        health.Hit(thisDamage, this.gameObject);
                    }
                }
            }
		}
	}

    void Update()
    {
        DebugExtension.DebugWireSphere(transform.position, Color.yellow, radius);
    }
}
