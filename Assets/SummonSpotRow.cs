using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonSpotRow : MonoBehaviour
{
    public SummonSpot[] summonSpots;
    public SummonSpotRow nextRow;
    public SummonSpotRow prevRow;

    int summonSpotCount;
    int emptySummonSpotCount;
    int filledSummonSpotCount;

    int nextFreeSummonSpot;

    public void Initialize()
    {
        int index = transform.GetSiblingIndex();
        if (index > 0)
        {
            prevRow = transform.parent.GetChild(index - 1).gameObject.GetComponent<SummonSpotRow>();
        }
        if (index < transform.parent.childCount - 1)
        {
            nextRow = transform.parent.GetChild(index + 1).gameObject.GetComponent<SummonSpotRow>();
        }

        summonSpots = GetComponentsInChildren<SummonSpot>();
        foreach(SummonSpot spot in summonSpots)
        {
            spot.rowIndex = index;
        }
    }

    void Update()
    {
        RefreshSummonSpotCount();
        Reorganize();
    }

    public int SummonSpotCount()
    {
        return summonSpots.Length;
    }

    public int EnemyCount()
    {
        return filledSummonSpotCount;
    }

    public void RefreshSummonSpotCount()
    {
        emptySummonSpotCount = 0;
        foreach (SummonSpot spot in summonSpots)
        {
            if (!spot.HasEnemy())
            {
                emptySummonSpotCount++;
            }
        }
        filledSummonSpotCount = SummonSpotCount() - emptySummonSpotCount;
    }

    private void Reorganize()
    {
        if (prevRow != null && filledSummonSpotCount > 0 && prevRow.emptySummonSpotCount >= filledSummonSpotCount)
        {
            foreach (SummonSpot spot in summonSpots)
            {
                if (spot.HasEnemy())
                {
                    SummonSpot previousRowSpot = prevRow.GetFreeSummonSpot();
                    if (previousRowSpot != null)
                    {
                        previousRowSpot.SetEnemy(spot.enemy);
                        //spot.SetEnemy(null);
                    }
                }
            }

            prevRow.RefreshSummonSpotCount();
            RefreshSummonSpotCount();
        }
    }

    public SummonSpot GetFreeSummonSpot()
    {
        if (emptySummonSpotCount == 0) { return null; }

        foreach (SummonSpot spot in summonSpots)
        {
            if (!spot.HasEnemy())
            {
                return spot;
            }
        }
        return null;
    }
}
