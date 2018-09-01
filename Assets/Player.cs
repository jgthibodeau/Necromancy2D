using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof(EntityController))]
public class Player : MonoBehaviour
{
    private EntityController controller;

    public bool triggerThrust, stickThrust;

    //private Tether tether;
    //private TetherCollider tetherCollider;
    public float normalDistance = 4f;
	public float distanceScale = 2f;

	void Start()
    {
        controller = GetComponent<EntityController>();
    }

	void Update() {
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
}
