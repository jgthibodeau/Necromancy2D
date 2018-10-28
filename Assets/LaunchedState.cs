using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchedState : FSMState
{
    protected override StateID GetID() { return StateID.Launched; }

    private Enemy enemy;
    private Vector3 launchForce;
    private EntityController controller;
    private AiController aiController;
    private Rigidbody2D rb;
    private bool launched = false;

    public GameObject explosionPrefab;
    
    void Awake()
    {
        enemy = GetComponent<Enemy>();
        controller = GetComponent<EntityController>();
        aiController = GetComponent<AiController>();
        rb = GetComponent<Rigidbody2D>();
    }

    public override void Reason(GameObject npc)
    {
    }

    public override void Act(GameObject npc)
    {
        rb.AddForce(launchForce);

        aiController.targetTransform = null;
        controller.Stop();
        controller.active = false;
        launched = true;
    }

    public void SetLaunchForce(Vector3 force)
    {
        launchForce = force;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (launched)
        {
            GameObject.Instantiate(explosionPrefab, transform.position, transform.rotation);
            GameObject.Destroy(enemy.gameObject);
        }
    }
}
