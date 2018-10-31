using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakSpotHealth : Health {
    public Collider2D[] weakColliders;
    public Collider2D[] normalColliders;

    public bool weakSpotEnabled;
    public float maxDamageWhileWeak;
    private float currentDamageWhileWeak;

    public float maxNonWeakHealth, currentNonWeakHealth;

    public override void Start()
    {
        base.Start();

        currentNonWeakHealth = maxNonWeakHealth;
    }
    public override void Hit(float damage, GameObject hitter)
    {
        if (weakSpotEnabled)
        {
            base.Hit(damage, hitter);
            currentDamageWhileWeak += damage;

            if (currentDamageWhileWeak > maxDamageWhileWeak)
            {
                DisableWeakSpot();
            }
        } else
        {
            if (currentNonWeakHealth > 0)
            {
                currentNonWeakHealth -= damage;
            }
            StartFlash();
        }
    }

    public bool IsWeak()
    {
        return currentNonWeakHealth <= 0;
    }

    public void EnableWeakSpot()
    {
        SetColliders(weakColliders, true);
        weakSpotEnabled = true;
        currentDamageWhileWeak = 0;
    }

    public void DisableWeakSpot()
    {
        SetColliders(weakColliders, false);
        weakSpotEnabled = false;
        currentNonWeakHealth = maxNonWeakHealth;
    }

    public void EnableNormalSpots()
    {
        SetColliders(normalColliders, true);
    }

    public void DisableNormalSpots()
    {
        SetColliders(normalColliders, false);
    }

    private void SetColliders(Collider2D[] colliders, bool enabled)
    {
        foreach(Collider2D c in colliders)
        {
            c.enabled = enabled;
        }
    }
}
