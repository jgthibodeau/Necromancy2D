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
    
    public float normalDistance = 4f;
	public float distanceScale = 2f;


    public float playerDamageShakeTime = 0.25f;
    public float playerDamageShakeScale = 0.1f;
    ScreenShake screenShake;

	void Start()
    {
        levelManager = LevelManager.instance;
        controller = GetComponent<EntityController>();
        health = GetComponent<Health>();
        screenShake = ScreenShake.instance;
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

    public void TakeDamage(float damage)
    {
        screenShake.Shake(playerDamageShakeTime, damage * playerDamageShakeScale);
    }

    public bool IsDead()
    {
        return health.IsDead();
    }
}
