using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchedBehavior : MonoBehaviour {

    public Enemy enemy;
    public Vector3 launchForce;
    private EntityController controller;
    private AiController aiController;
    private Rigidbody2D rb;
    private bool launched = false;

    public GameObject explosionPrefab;

    void Awake()
    {
        controller = GetComponent<EntityController>();
        aiController = GetComponent<AiController>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void DoBehavior()
    {
        rb.AddForce(launchForce);

        aiController.targetTransform = null;
        controller.Stop();
        controller.active = false;
        launched = true;
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
