using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class WeaponCollider : MonoBehaviour {
    public float damage;
    public LayerMask layerMask;
    Collider2D collider;

    List<Health> alreadyHit;

    void Start()
    {
        collider = GetComponent<Collider2D>();
        collider.enabled = false;
        collider.isTrigger = true;
        alreadyHit = new List<Health>();
    }

	public void Activate(float damage, LayerMask layerMask)
    {
        alreadyHit = new List<Health>();
        this.damage = damage;
        this.layerMask = layerMask;
        collider.enabled = true;
    }

    public void DeActivate()
    {
        collider.enabled = false;
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        Health health = other.gameObject.GetComponentInParent<Health>();
        if (health != null && !alreadyHit.Contains(health) && Util.InLayerMask(other.gameObject.layer, layerMask))
        {
            alreadyHit.Add(health);
            health.Hit(damage, gameObject);
        }
    }
}
