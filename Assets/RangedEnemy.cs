using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : Enemy
{
    public Vector3 playerVelocity;

    public bool leadPlayer;
    public float updatePlayerVelocityRate;
    
    public GameObject friendlyProjectile;
    public GameObject enemyProjectile;
    public Transform firePosition;
    public float fireRandomness;

    public int friendlyFireballLayer;
    public int enemyFireballLayer;

    public bool aimAtMouse = true;

    public override void Start()
    {
        base.Start();

        friendlyFireballLayer = LayerMask.NameToLayer("FriendlyProjectile");
        enemyFireballLayer = LayerMask.NameToLayer("EnemyProjectile");
    }

    public override void StartAttack()
    {
        Vector2 aimPoint;
        GameObject bullet;
        //if (lifeState == LIFE_STATE.ALIVE)
        if (fsm.CurrentStateID == StateID.Chase)
        {
            bullet = enemyProjectile;
            aimPoint = CalculateAimPoint(bullet);
        }
        else
        {
            bullet = friendlyProjectile;
            if (aimAtMouse)
            {
                aimPoint = summonCircle.transform.position;
            }
            else
            {
                aimPoint = firePosition.position + transform.up;
            }
        }
        
        FireAt(aimPoint, bullet);
    }

    public override void StopAttack()
    {
    }

    void FireAt(Vector2 point, GameObject bullet)
    {
        Vector2 position = (Vector2)firePosition.position + Random.insideUnitCircle * fireRandomness;
        Quaternion rotation = Quaternion.LookRotation((point - position), Vector3.forward);
        GameObject bulletInst = GameObject.Instantiate(bullet, position, rotation);
        //Bullet b = bulletInst.GetComponent<Bullet>();
        //bulletInst.layer = layer;
    }


    Vector2 CalculateAimPoint(GameObject bullet)
    {
        Vector2 totarget = player.transform.position - transform.position;

        float bulletSpeed = bullet.GetComponent<Bullet>().bulletForce + rigidBody.velocity.magnitude;

        float a = Vector2.Dot(playerVelocity, playerVelocity) - (bulletSpeed * bulletSpeed);
        float b = 2 * (playerVelocity.x * (player.transform.position.x - transform.position.x) + playerVelocity.y * (player.transform.position.y - transform.position.y));
        float c = Vector2.Dot(totarget, totarget);

        Debug.Log(bulletSpeed + " " + a + " " + b + " " + c);

        //float p = -b / (2 * a);
        //float disc = b * b - 4 * a * c;
        //float t1 = (-b + Mathf.Sqrt(disc)) / (2 * a);

        float p = -b / (2 * a);
        float q = (float)Mathf.Sqrt((b * b) - 4 * a * c) / (2 * a);

        float t1 = p - q;
        float t2 = p + q;
        float t;

        if (t1 > t2 && t2 > 0)
        {
            t = t2;
        }
        else if (t2 > t1 && t1 > 0)
        {
            t = t1;
        }
        else
        {
            t = 0;
        }

        return player.transform.position + playerVelocity * t;
        //Vector bulletPath = aimSpot - tower.position;
        //float timeToImpact = bulletPath.Length() / bullet.speed;//speed must be in units per second
    }

    static float PredictiveAimWildGuessAtImpactTime()
    {
        return Random.Range(1, 5);
    }

    static public bool PredictiveAim(Vector3 muzzlePosition, float projectileSpeed, Vector3 targetPosition, Vector3 targetVelocity, out Vector2 projectileVelocity)
    {
        Debug.Assert(projectileSpeed > 0, "What are you doing shooting at something with a projectile that doesn't move?");
        if (muzzlePosition == targetPosition)
        {
            //Why dost thou hate thyself so?
            //Do something smart here. I dunno... whatever.
            projectileVelocity = projectileSpeed * (Random.rotation * Vector2.up);
            return true;
        }

        //Much of this is geared towards reducing floating point precision errors
        float projectileSpeedSq = projectileSpeed * projectileSpeed;
        float targetSpeedSq = targetVelocity.sqrMagnitude; //doing this instead of self-multiply for maximum accuracy
        float targetSpeed = Mathf.Sqrt(targetSpeedSq);
        Vector3 targetToMuzzle = muzzlePosition - targetPosition;
        float targetToMuzzleDistSq = targetToMuzzle.sqrMagnitude; //doing this instead of self-multiply for maximum accuracy
        float targetToMuzzleDist = Mathf.Sqrt(targetToMuzzleDistSq);
        Vector3 targetToMuzzleDir = targetToMuzzle;
        targetToMuzzleDir.Normalize();

        Vector3 targetVelocityDir = targetVelocity;
        targetVelocityDir.Normalize();

        //Law of Cosines: A*A + B*B - 2*A*B*cos(theta) = C*C
        //A is distance from muzzle to target (known value: targetToMuzzleDist)
        //B is distance traveled by target until impact (targetSpeed * t)
        //C is distance traveled by projectile until impact (projectileSpeed * t)
        float cosTheta = Vector3.Dot(targetToMuzzleDir, targetVelocityDir);

        bool validSolutionFound = true;
        float t;
        if (Mathf.Approximately(projectileSpeedSq, targetSpeedSq))
        {
            //a = projectileSpeedSq - targetSpeedSq = 0
            //We want to avoid div/0 that can result from target and projectile traveling at the same speed
            //We know that C and B are the same length because the target and projectile will travel the same distance to impact
            //Law of Cosines: A*A + B*B - 2*A*B*cos(theta) = C*C
            //Law of Cosines: A*A + B*B - 2*A*B*cos(theta) = B*B
            //Law of Cosines: A*A - 2*A*B*cos(theta) = 0
            //Law of Cosines: A*A = 2*A*B*cos(theta)
            //Law of Cosines: A = 2*B*cos(theta)
            //Law of Cosines: A/(2*cos(theta)) = B
            //Law of Cosines: 0.5f*A/cos(theta) = B
            //Law of Cosines: 0.5f * targetToMuzzleDist / cos(theta) = targetSpeed * t
            //We know that cos(theta) of zero or less means there is no solution, since that would mean B goes backwards or leads to div/0 (infinity)
            if (cosTheta > 0)
            {
                t = 0.5f * targetToMuzzleDist / (targetSpeed * cosTheta);
            }
            else
            {
                validSolutionFound = false;
                t = PredictiveAimWildGuessAtImpactTime();
            }
        }
        else
        {
            //Quadratic formula: Note that lower case 'a' is a completely different derived variable from capital 'A' used in Law of Cosines (sorry):
            //t = [ -b � Sqrt( b*b - 4*a*c ) ] / (2*a)
            float a = projectileSpeedSq - targetSpeedSq;
            float b = 2.0f * targetToMuzzleDist * targetSpeed * cosTheta;
            float c = -targetToMuzzleDistSq;
            float discriminant = b * b - 4.0f * a * c;

            if (discriminant < 0)
            {
                //Square root of a negative number is an imaginary number (NaN)
                //Special thanks to Rupert Key (Twitter: @Arakade) for exposing NaN values that occur when target speed is faster than or equal to projectile speed
                validSolutionFound = false;
                t = PredictiveAimWildGuessAtImpactTime();
            }
            else
            {
                //a will never be zero because we protect against that with "if (Mathf.Approximately(projectileSpeedSq, targetSpeedSq))" above
                float uglyNumber = Mathf.Sqrt(discriminant);
                float t0 = 0.5f * (-b + uglyNumber) / a;
                float t1 = 0.5f * (-b - uglyNumber) / a;
                //Assign the lowest positive time to t to aim at the earliest hit
                t = Mathf.Min(t0, t1);
                if (t < Mathf.Epsilon)
                {
                    t = Mathf.Max(t0, t1);
                }

                if (t < Mathf.Epsilon)
                {
                    //Time can't flow backwards when it comes to aiming.
                    //No real solution was found, take a wild shot at the target's future location
                    validSolutionFound = false;
                    t = PredictiveAimWildGuessAtImpactTime();
                }
            }
        }

        //Vb = Vt - 0.5*Ab*t + [(Pti - Pbi) / t]
        projectileVelocity = targetVelocity + (-targetToMuzzle / t);
        if (!validSolutionFound)
        {
            //PredictiveAimWildGuessAtImpactTime gives you a t that will not result in impact
            // Which means that all that math that assumes projectileSpeed is enough to impact at time t breaks down
            // In this case, we simply want the direction to shoot to make sure we
            // don't break the gameplay rules of the cannon's capabilities aside from gravity compensation
            projectileVelocity = projectileSpeed * projectileVelocity.normalized;
        }

        //FOR CHECKING ONLY (valid only if gravity is 0)...
        //float calculatedprojectilespeed = projectileVelocity.magnitude;
        //bool projectilespeedmatchesexpectations = (projectileSpeed == calculatedprojectilespeed);
        //...FOR CHECKING ONLY

        return validSolutionFound;
    }

    IEnumerator RefreshPlayerVelocity()
    {
        while (true)
        {
            playerVelocity = player.GetComponent<Rigidbody2D>().velocity;
            yield return new WaitForSeconds(updatePlayerVelocityRate);
        }
    }
}
