using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boundary : MonoBehaviour {
    public int killTime = 10;

    void OnTriggerEnter2D(Collider2D other)
    {
        Kill k = other.gameObject.GetComponent<Kill>();

        if (k != null)
        {
            k.outOfBounds = true;
            StartCoroutine(k.KillInFuture(k, killTime));
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Kill k = other.gameObject.GetComponent<Kill>();

        if (k != null)
        {
            k.outOfBounds = false;
        }
    }
}
