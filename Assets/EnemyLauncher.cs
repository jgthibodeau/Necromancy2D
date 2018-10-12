using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLauncher : MonoBehaviour {
    public LaunchLineRenderer launchLineRenderer;

    public Enemy enemyToLaunch;
    public float launchSpeedScale;
    public LayerMask launchableLayers;

    private Vector3 mouse;
    public float mouseRadius = 5f;

    public float distance;

    public float gradientChangeDistance1, gradientChangeDistance2;
    public Gradient smallForce;
    public Gradient medForce;
    public Gradient bigForce;

    [Range(0, 1)]
    public float launchTimeScale;

    void Update()
    {
        mouse = Util.MouseInWorld();
        DebugExtension.DebugWireSphere(mouse, Color.yellow, mouseRadius);

        if (enemyToLaunch != null)
        {
            Time.timeScale = launchTimeScale;
            distance = Vector2.Distance(enemyToLaunch.transform.position, mouse);
        }
        else
        {
            Time.timeScale = 1f;
            distance = 0;
        }
        Time.fixedDeltaTime = 0.02F * Time.timeScale;

        UpdateGraphics();
    }

    public void SetEnemyToLaunch()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(mouse, mouseRadius, launchableLayers);
        Collider2D collider = null;
        float distance = Mathf.Infinity;
        foreach(Collider2D c in colliders)
        {
            float currentDistance = Vector2.Distance(mouse, c.transform.position);
            if (currentDistance < distance)
            {
                distance = currentDistance;
                collider = c;
            }
        }
        if (collider != null)
        {
            enemyToLaunch = collider.GetComponent<Enemy>();
        }

        //Collider2D collider = Physics2D.OverlapPoint(mouse, launchableLayers);
        //if (collider != null)
        //{
        //    enemyToLaunch = collider.GetComponent<Enemy>();
        //}
    }

    public void LaunchEnemy()
    {
        if (enemyToLaunch != null)
        {
            Vector2 force = (enemyToLaunch.transform.position - mouse) * launchSpeedScale;
            enemyToLaunch.Launch(force);
            enemyToLaunch = null;
        }
    }

    [Range(0, 1)]
    public float enemyPointScale;
    public void UpdateGraphics()
    {
        Gradient gradient;
        if (distance < gradientChangeDistance1)
        {
            float percent = Util.ConvertScale(0, gradientChangeDistance1, 0, 1, distance);
            gradient = Util.Lerp(smallForce, medForce, percent);
        } else
        {
            float percent = Util.ConvertScale(gradientChangeDistance1, gradientChangeDistance2, 0, 1, distance);
            gradient = Util.Lerp(medForce, bigForce, percent);
        }
        
        if (enemyToLaunch != null)
        {
            //Vector3 enemyPoint = enemyToLaunch.transform.position;
            Vector3 enemyPoint = ((enemyToLaunch.transform.position - mouse) * enemyPointScale + enemyToLaunch.transform.position);
            launchLineRenderer.Set(mouse, enemyPoint, gradient);
        } else
        {
            launchLineRenderer.Reset();
        }
    }
}
