using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof(EntityController))]
[RequireComponent(typeof(Health))]
public class Player : MonoBehaviour
{
    private LevelManager levelManager;
    private EntityController controller;
    private Health health;

    public bool triggerThrust, stickThrust;

    //private Tether tether;
    //private TetherCollider tetherCollider;
    public float normalDistance = 4f;
	public float distanceScale = 2f;

	void Start()
    {
        levelManager = LevelManager.instance;
        controller = GetComponent<EntityController>();
        health = GetComponent<Health>();
    }

    void Update()
    {
        if (!health.IsDead())
        {
            MovePlayer();
        } else
        {
            KillPlayer();
        }
    }

    void MovePlayer()
    {
        Vector2 moveDirection = new Vector2(Util.GetAxis("Horizontal"), Util.GetAxis("Vertical"));
        controller.moveDirection = moveDirection;
        if (triggerThrust)
        {
            controller.thrust = Util.GetAxis("Thrust");
        }
        else if (stickThrust)
        {
            controller.thrust = Mathf.Clamp01(moveDirection.magnitude);
        }
    }

    void KillPlayer()
    {
        controller.thrust = 0f;
        levelManager.OnPlayerDeath();
    }
}
