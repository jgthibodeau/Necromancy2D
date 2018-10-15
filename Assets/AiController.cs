using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class AiController : MonoBehaviour
{
    public enum FOLLOW_STATE { FAR, MID, CLOSE }
    public FOLLOW_STATE followState = FOLLOW_STATE.FAR;

    public Transform targetTransform;

    public float distanceToTarget;

    public float minDistance;
    public float maxDistance;
    public float desiredDistance;
    public float speedScaleDistance;

    public float speed;
    public float turnSpeed;
    
    private EntityController controller;
    private Path path;
    private int currentWayPoint = 0;
    Seeker seeker;
    Vector2 aStarTarget;

    void Awake()
    {
        controller = GetComponent<EntityController>();
        seeker = GetComponent<Seeker>();
    }

    void Start()
    {
        aStarTarget = transform.position;
        UpdatePath();
        StartCoroutine(UpdatePathPoint());
    }
    
    void FixedUpdate()
    {
        if (targetTransform == null)
        {
            return;
        }

        Vector2 target = targetTransform.position;
        Vector2 direction = target - (Vector2)transform.position;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, controller.avoidanceDistance, controller.avoidanceLayers);
        if (hit.collider != null)
        {
            Debug.Log(gameObject.name + " hit " + hit.collider.gameObject.name + ", falling back to A*");
            target = aStarTarget;
            direction = target - (Vector2)transform.position;
        }

        Debug.DrawRay(transform.position, direction, Color.red);
        distanceToTarget = Vector3.Distance(target, transform.position);

        controller.active = true;

        if (distanceToTarget > maxDistance)
        {
            followState = FOLLOW_STATE.FAR;
        }
        else if (distanceToTarget > minDistance)
        {
            followState = FOLLOW_STATE.MID;
        }
        else if (distanceToTarget <= minDistance)
        {
            followState = FOLLOW_STATE.CLOSE;
        }

        Vector2 moveDirection = Vector2.zero;
        float speedScale = 1f;
        switch (followState)
        {
            case FOLLOW_STATE.FAR:
                moveDirection = direction;
                break;
            case FOLLOW_STATE.MID:
                moveDirection = direction;

                //float scale = Util.ConvertScale(minDistance, maxDistance, 0, 1, distanceToTarget);

                if (distanceToTarget > desiredDistance)
                {
                    speedScale = Util.ConvertScale(desiredDistance, maxDistance, 0, 1, distanceToTarget);
                }
                else
                {
                    speedScale = -Util.ConvertScale(minDistance, desiredDistance, 0, 1, distanceToTarget);
                }

                break;
            case FOLLOW_STATE.CLOSE:
                controller.Stop();
                controller.active = false;
                break;
        }

        controller.speedScale = speedScale;
        controller.SetMoveDirection(moveDirection);
    }

    void UpdatePath()
    {
        if (targetTransform != null)
        {
            seeker.StartPath(transform.position, targetTransform.position, OnPathComplete);
        }
    }

    public void OnPathComplete(Path p)
    {
        currentWayPoint = 0;
        if (p.error)
        {
            path = null;
            Debug.LogError(p.errorLog);
        }
        else
        {
            path = p;
        }
    }

    float nextWaypointDistance = 3;
    IEnumerator UpdatePathPoint()
    {
        while (true)
        {
            if (seeker.IsDone())
            {
                UpdatePath();
            }
            if (path != null)
            {
                float distanceToWaypoint = Vector3.Distance(transform.position, path.vectorPath[currentWayPoint]);
                if (distanceToWaypoint < nextWaypointDistance)
                {
                    // Check if there is another waypoint or if we have reached the end of the path
                    if (currentWayPoint + 1 < path.vectorPath.Count)
                    {
                        currentWayPoint++;
                        aStarTarget = path.vectorPath[currentWayPoint];
                    }
                }
            }
            else
            {
                aStarTarget = transform.position;
            }

            yield return null;
        }
    }
}
