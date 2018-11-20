using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAnimator : MonoBehaviour {
    private Enemy enemy;
	
    void Start()
    {
        enemy = GetComponentInParent<Enemy>();
    }

    public void TelegraphAttack()
    {
        enemy.TelegraphAttack();
    }

    public void StartAttack()
    {
        enemy.StartAttack();
    }

    public void StopAttack()
    {
        enemy.StopAttack();
    }

    public void PreJump()
    {
        enemy.PreJump();
    }

    public void Jump()
    {
        enemy.Jump();
    }

    public void StartLand()
    {
        enemy.Land();
    }

    public void StopLand()
    {
        enemy.StopLand();
    }
}
