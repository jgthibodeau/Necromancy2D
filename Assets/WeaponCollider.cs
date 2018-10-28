using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class WeaponCollider : MonoBehaviour {
    public float damage;
    public LayerMask layerMask;
    Collider2D collider;
    public float radius;

    public bool canHitMultiple;

    public List<Health> alreadyHit = new List<Health>();

    bool active;

    void Start()
    {
        collider = GetComponent<Collider2D>();
        collider.enabled = false;
        collider.isTrigger = true;
    }

    void Update()
    {
        //if (active)
        //{
        //    foreach (Collider2D collider in Physics2D.OverlapCircleAll(transform.position, radius, layerMask))
        //    {
        //        bool hit = HitCollider(collider);
        //        if (hit && !canHitMultiple)
        //        {
        //            DeActivate();
        //            break;
        //        }
        //    }
        //}
    }

	public void Activate(float damage, LayerMask layerMask)
    {
        alreadyHit.Clear();
        this.damage = damage;
        this.layerMask = layerMask;
        collider.enabled = true;
        //active = true;
    }

    public void DeActivate()
    {
        collider.enabled = false;
        //active = false;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        HitCollider(collider);
    }

    bool HitCollider(Collider2D collider)
    {
        Health health = collider.gameObject.GetComponentInParent<Health>();
        Debug.Log("Hitting " + collider + " " + (health != null) + " " + !alreadyHit.Contains(health));
        if (health != null && !alreadyHit.Contains(health) && Util.InLayerMask(collider.gameObject.layer, layerMask))
            //if (health != null && !alreadyHit.Contains(health))
        {
            Debug.Log("Hit " + collider);
            alreadyHit.Add(health);
            health.Hit(damage, gameObject);

            if (!canHitMultiple)
            {
                DeActivate();
            }

            return true;
        }
        else
        {
            Debug.Log("Can't hit " + collider);
            return false;
        }
    }
}
