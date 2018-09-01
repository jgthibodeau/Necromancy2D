using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Capturable : MonoBehaviour {
    Rigidbody2D rb;
    public GravityWell gravityWell;

    public bool isCaptured;
    public float capturedDrag = 2;
    public float regularDrag = 0.5f;

    public bool rotateWithGravity;

    private int originalLayer;

    public void SetCaptured(bool newCaptured, int capturedLayer, GravityWell g)
    {
        if (newCaptured && !isCaptured)
        {
            gameObject.layer = capturedLayer;
            gravityWell = g;
        } else if (!newCaptured && isCaptured)
        {
            gameObject.layer = originalLayer;
        }

        isCaptured = newCaptured;
    }

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();
        originalLayer = gameObject.layer;
    }
	
	// Update is called once per frame
	void Update () {
        rb.drag = isCaptured ? capturedDrag : regularDrag;
    }

    public Rigidbody2D GetRigidbody()
    {
        return rb;
    }
}
