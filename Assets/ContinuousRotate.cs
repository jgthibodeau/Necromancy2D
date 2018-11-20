using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinuousRotate : MonoBehaviour {
    public Vector3 speed;
    
	// Update is called once per frame
	void Update () {
        transform.eulerAngles = transform.eulerAngles + speed * Time.deltaTime;
	}
}
