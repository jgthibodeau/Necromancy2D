using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeRotation : MonoBehaviour {
    public bool x;
    public bool y;
    public bool z;

    private Vector3 original;

    // Use this for initialization
    void Start () {
        original = transform.eulerAngles;
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 current = transform.eulerAngles;
        if (x)
        {
            current.x = original.x;
        }
        if (y)
        {
            current.y = original.y;
        }
        if (z)
        {
            current.z = original.z;
        }

        transform.eulerAngles = current;
	}
}
