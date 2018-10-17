using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAnimator : MonoBehaviour {
    private Enemy enemy;
	
    void Start()
    {
        enemy = GetComponentInParent<Enemy>();
    }

    public void StartAttack()
    {
        enemy.StartAttack();
    }
	
    public void StopAttack()
    {
        enemy.StopAttack();
    }
}
