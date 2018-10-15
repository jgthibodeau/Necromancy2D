using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchedBehavior : MonoBehaviour {

    public Enemy enemy;
    public bool launched;
    public Vector3 launchForce;
    private EntityController controller;
    private AiController aiController;
    private Rigidbody2D rigidbody2D;

    public GameObject explosionPrefab;

    void Awake()
    {
        controller = GetComponent<EntityController>();
        aiController = GetComponent<AiController>();
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    public void DoBehavior()
    {
        if (launched)
        {
            rigidbody2D.AddForce(launchForce);

            aiController.targetTransform = null;
            controller.Stop();
            controller.active = false;
        }
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
