﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    private LevelManager levelManager;
    public bool invincible = false;
    public float maxHealth = 100;
    public float resurrectHealth = 50;
	public float currentHealth;

    public float lowDamageThreshold;
    public float lowDamageScale;

    public int score;

    public GameObject[] alwaysSpawnOnDeath;
    public GameObject spawnOnDeath;
    [Range(0f, 1f)]
    public float spawnChance;
    [Range(0f, 1f)]
    public float spawnChanceWhenPlayerLowHealth;
    //public bool keepDeathRotation;
    //public bool keepDeathScale;
    public Vector3 minSpawnForce = new Vector3 (-1, 1, -1);
    public Vector3 maxSpawnForce = new Vector3(1, 2, 1);

    public bool destroyOnDeath = true;

    private bool alreadyDead = false;

    void Start() {
        //		Reset ();
        //		respawnable = GetComponent<Respawnable> ();
        levelManager = LevelManager.instance;
    }

	public void Hit(float damage, GameObject hitter) {
        TakeDamage(damage);
	}
    
	public void Heal(float amount) {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
	}
    
	public void TakeDamage(float damage) {
        if (IsDead())
        {
            return;
        }

        if (gameObject.tag == "Player")
        {
            gameObject.GetComponent<Player>().TakeDamage(damage);
        }

        if (invincible)
        {
            return;
        }

        if (currentHealth < lowDamageThreshold)
        {
            damage *= lowDamageScale;
        }
		currentHealth -= damage;
        
		if (IsDead ()) {
			Kill ();
		}
	}

	public bool IsDead() {
		return alreadyDead || currentHealth <= 0;
	}

	public void Kill() {
        alreadyDead = true;

        Debug.Log ("Killed " + gameObject);

        foreach(Collider c in GetComponentsInChildren<Collider>())
        {
            c.enabled = false;
        }
        
        if (destroyOnDeath)
        {
            GameObject.Destroy(gameObject);
        }

        foreach (GameObject go in alwaysSpawnOnDeath)
        {
            SpawnDeathObject(go);
        }
        float chance = MyGameManager.instance.GetPlayer().IsCriticalHealth() ? spawnChanceWhenPlayerLowHealth : spawnChance;
        if (Random.value < chance)
        {
            SpawnDeathObject(spawnOnDeath);
        }

        levelManager.AddScore(score);
    }

    public void SpawnDeathObject(GameObject go)
    {
        if (go == null)
        {
            return;
        }

        GameObject inst = GameObject.Instantiate(go, transform.position + Vector3.up * 0.1f, transform.rotation);

        if (minSpawnForce.magnitude > 0 && maxSpawnForce.magnitude > 0)
        {
            Rigidbody[] rbs = inst.GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody rb in rbs)
            {
                Vector3 force = new Vector3(
                    Random.Range(minSpawnForce.x, maxSpawnForce.x),
                    Random.Range(minSpawnForce.y, maxSpawnForce.y),
                    Random.Range(minSpawnForce.z, maxSpawnForce.z)
                    );
                rb.AddForce(force);
            }
        }
    }

	public void Reset() {
		currentHealth = maxHealth;
	}

	public float Percentage() {
		return currentHealth / maxHealth;
	}
}
