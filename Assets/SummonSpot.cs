using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonSpot : MonoBehaviour {
    public Enemy enemy;
    public int spotIndex;
    public int rowIndex;

    private Vector3 originalLocalPosition;

    public bool attacking;
    public float xOffsetScale;
    public float yOffsetScale;

    public void SetEnemy(Enemy e)
    {
        if (e != null)
        {
            this.enemy = e;
            e.SetSummonSpot(this);
        } else
        {
            this.enemy = null;
        }
    }

    public bool HasEnemy()
    {
        return enemy != null;// && enemy.gameObject != null;
    }

    void Start()
    {
        originalLocalPosition = transform.localPosition;

        GetComponent<Renderer>().enabled = false;

        //currentFormation = frontFormation;
    }

    void Update()
    {
        //if (attacking)
        //{
        //    Vector3 localPosition = originalLocalPosition;
        //    localPosition.y += (rowIndex + 1) * yOffsetScale;
        //    localPosition.x -= spotIndex * xOffsetScale;
        //    transform.localPosition = localPosition;
        //    //transform.position = currentFormation.attackPosition.position;
        //} else
        //{
        //    transform.localPosition = originalLocalPosition;
        //    //transform.position = currentFormation.normalPosition.position;
        //}
    }
}
