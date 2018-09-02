using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    private LevelManager levelManager;
    public bool invincible = false;
    public float maxHealth = 100;
	public float currentHealth;

    public int score;

    public GameObject deathParticles;
    public GameObject spawnOnDeath;
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

	private float previouslyHealedHealth;
	public bool Heal(float amount) {
		bool healing = (currentHealth < maxHealth) && (previouslyHealedHealth <= currentHealth);

		if (healing) {
			currentHealth = Mathf.Clamp (currentHealth + amount, 0, maxHealth);
		}

		previouslyHealedHealth = currentHealth;

		return healing;
	}
    
	public void TakeDamage(float damage) {
        if (invincible || IsDead())
        {
            return;
        }

		Debug.Log ("Taking damage " + damage + " " + currentHealth + " " + gameObject);
		currentHealth -= damage;
		Debug.Log ("Took damage " + damage + " " + currentHealth + " " + gameObject);
        
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

        //StartCoroutine("KillAndSpawn");
        if (destroyOnDeath)
        {
            GameObject.Destroy(gameObject);
        }

        SpawnDeathObject();

        levelManager.AddScore(score);
    }

    IEnumerator KillAndSpawn()
    {
        yield return null;
        Debug.Break();
        yield return null;
        Debug.Break();

        GameObject.Destroy(gameObject);

        SpawnDeathObject();
    }

    public void SpawnDeathObject()
    {
        if (spawnOnDeath == null)
        {
            return;
        }

        GameObject inst = GameObject.Instantiate(spawnOnDeath, transform.position + Vector3.up * 0.1f, transform.rotation);

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
