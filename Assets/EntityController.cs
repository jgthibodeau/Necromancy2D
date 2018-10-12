using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EntityController : MonoBehaviour
{
    public Transform graphicsBase;
    public Transform graphics;

    public bool active = true;

    public float velocity;

	public float speed = 6f;
    public float turnSpeed = 5f;

	private Rigidbody2D rb;

    private Vector2 moveDirection = Vector2.zero;
    private Quaternion originalGraphicsBaseRot;
    private Vector3 originalGraphicsLocalRot;
    private Quaternion axisTilt;

    public void SetMoveDirection(Vector2 newDir)
    {
        moveDirection = Vector3.Lerp(moveDirection, newDir, turnSpeed * Time.deltaTime);
        moveDirection = Vector3.ClampMagnitude(moveDirection, 1);
    }

	void Start()
    {
        originalGraphicsBaseRot = graphicsBase.rotation;
        originalGraphicsLocalRot = graphics.localEulerAngles;
        axisTilt = graphics.rotation;

        rb = GetComponent<Rigidbody2D>();
    }

    public void Stop()
    {
        this.moveDirection = Vector2.zero;
        this.speed = 0;
    }

    void FixedUpdate()
    {
        Move();
    }

    public void Move()
    {
        if (!active)
        {
            return;
        }

        Debug.DrawRay(transform.position, moveDirection, Color.cyan);

        //velocity = rb.velocity.magnitude;
        //float angle = Vector3.SignedAngle(Vector3.up, moveDirection, Vector3.forward);
        //rb.MoveRotation(Mathf.LerpAngle(rb.rotation, angle, turnSpeed * Time.deltaTime));
        //rb.MovePosition(rb.position + moveDirection * speed * Time.fixedDeltaTime);

        rb.velocity = (moveDirection * Time.fixedDeltaTime * speed);
        //rb.AddForce(moveDirection * speed);
    }

    public void RotateTowardsMotion()
    {
        Vector3 velocity = GetComponent<Rigidbody2D>().velocity;
        if (velocity.magnitude > 0)
        {
            RotateTowards(velocity);
        }
    }

    public void RotateAwayFrom(Transform other)
    {
        Vector3 vectorToTarget = transform.position - other.position;
        RotateTowards(vectorToTarget);
    }

    public void RotateTowards(Transform other)
    {
        Vector3 vectorToTarget = other.position - transform.position;
        RotateTowards(vectorToTarget);
    }

    public void RotateTowards(Vector3 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * turnSpeed);

        //rotate graphics z only
        //graphics.eulerAngles = originalGraphicsRot;
        //Vector3 newGraphicsRot = graphics.localEulerAngles;
        //newGraphicsRot.y = transform.eulerAngles.z;
        //graphics.localEulerAngles = newGraphicsRot;

        //graphicsRot.y = transform.eulerAngles.z;
        //graphics.eulerAngles = graphicsRot;

        //graphics.localRotation = Quaternion.Euler(0, transform.eulerAngles.z, 0) * axisTilt;

        //Vector3 newLocal = originalGraphicsLocalRot;
        //newLocal.y = transform.eulerAngles.z;
        //graphics.localEulerAngles = newLocal;

        //Vector3 newRot = graphics.eulerAngles;
        //newRot.z = 0;
        //graphics.eulerAngles = newRot;

        graphicsBase.rotation = originalGraphicsBaseRot;
        originalGraphicsLocalRot.y = -transform.eulerAngles.z;
        graphics.localEulerAngles = originalGraphicsLocalRot;
    }

}
