using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonSpotGroup : MonoBehaviour {
    public float moveSpeed;

    public SummonSpotRow[] summonSpotRows;
    private List<SummonSpot> summonSpots = new List<SummonSpot>();
    private int numberSpots;

    public SummonSpotRow[] GetSummonSpotRows()
    {
        return summonSpotRows;
    }

    public int SummonSpotCount()
    {
        return numberSpots;
    }

    public SummonSpot GetFreeSummonSpot()
    {
        //foreach (SummonSpotRow row in summonSpotRows)
        //{
        //    SummonSpot spot = row.GetFreeSummonSpot();
        //    if (spot != null)
        //    {
        //        return spot;
        //    }
        //}
        //return null;
        foreach(SummonSpot spot in summonSpots)
        {
            if (!spot.HasEnemy())
            {
                return spot;
            }
        }
        return null;
    }

    public int FilledSpotCount()
    {
        int count = 0;
        foreach (SummonSpotRow row in summonSpotRows)
        {
            count += row.EnemyCount();
        }
        return count;
    }

    public int EmptySpotCount()
    {
        int count = 0;
        foreach (SummonSpotRow row in summonSpotRows)
        {
            count += row.GetEmptySummonSpotCount();
        }
        return count;
    }

    public void SetEnemy(int i, Enemy enemy)
    {
        SetEnemy(summonSpots[i], enemy);
    }

    public void SetEnemy(SummonSpot spot, Enemy enemy)
    {
        spot.SetEnemy(enemy);
        if (enemy != null)
        {
            enemy.SetAllySpeed(moveSpeed);
        }
    }

    public void MoveTo(SummonSpotGroup newGroup, bool attack)
    {
        for(int i=0; i<summonSpots.Count; i++)
        {
            Enemy enemy = null;
            if (summonSpots[i].HasEnemy())
            {
                enemy = summonSpots[i].enemy;
                enemy.SetAllyAttack(attack);
            }
            newGroup.SetEnemy(i, enemy);
        }

        //int newRowIndex = 0;
        //int newSpotIndex = 0;
        
        //SummonSpotRow[] newRows = newGroup.GetSummonSpotRows();
        //SummonSpotRow newRow = summonSpotRows[0];

        //foreach (SummonSpotRow oldRow in summonSpotRows)
        //{
        //    foreach (SummonSpot oldSpot in oldRow.summonSpots)
        //    {
        //        if (oldSpot.HasEnemy())
        //        {
        //            Enemy enemy = oldSpot.enemy;
        //            enemy.allyBehavior.attacking = attack;
        //            //oldSpot.SetEnemy(null);

        //            if (newSpotIndex >= newRow.SummonSpotCount())
        //            {
        //                newRowIndex++;
        //                newRow = newRows[newRowIndex];
        //                newSpotIndex = 0;
        //            }
        //            newRow.summonSpots[newSpotIndex].SetEnemy(enemy);
        //            newSpotIndex++;
        //        }
        //    }
        //}
    }

    public bool Summon(Enemy enemy)
    {
        if (enemy != null)
        {
            SummonSpot summonSpot = GetFreeSummonSpot();
            if (summonSpot != null)
            {
                SetEnemy(summonSpot, enemy);
                enemy.Resurrect(summonSpot);
                summonSpot.enemy = enemy;
                return true;
            }
        }
        return false;
    }
    
	public void Initialize () {
        numberSpots = 0;
        summonSpotRows = GetComponentsInChildren<SummonSpotRow>();
        foreach(SummonSpotRow row in summonSpotRows)
        {
            row.Initialize();
            numberSpots += row.SummonSpotCount();

            foreach (SummonSpot spot in row.summonSpots)
            {
                summonSpots.Add(spot);
            }
        }
    }
}
