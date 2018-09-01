using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EntityController : MonoBehaviour {
    public float velocity;

	public float speed = 6.0f;
    public float normalTurnSpeed = 5;
    public float thrustTurnSpeed = 2;

	private Rigidbody2D rb;

    public Vector2 moveDirection = Vector2.zero;
    public float thrust = 0;

	void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Stop()
    {
        this.moveDirection = Vector2.zero;
    }

    void FixedUpdate()
    {
        Debug.DrawRay(transform.position, moveDirection, Color.cyan);

        velocity = rb.velocity.magnitude;

        float turnSpeed = normalTurnSpeed;
        if (thrust > 0)
        {
            turnSpeed = thrustTurnSpeed;
            Vector3 force = transform.up * thrust * speed;
            rb.AddForce(force, ForceMode2D.Force);
        }

        if (moveDirection.magnitude > 0)
        {
            moveDirection = Vector3.ClampMagnitude(moveDirection, 1);
            float angle = Vector3.SignedAngle(Vector3.up, moveDirection, Vector3.forward);
            rb.MoveRotation(Mathf.LerpAngle(rb.rotation, angle, turnSpeed * Time.deltaTime));
        }
    }
}
