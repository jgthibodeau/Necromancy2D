using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class Camera2DSmoothFollow : MonoBehaviour {
    private Camera camera;

    public Rigidbody2D player;
    public float speed;

    public float minSpeedZoom;
    public float maxSpeedZoom;
    public float minSize;
    public float maxSize;
    public float zoomRate;

    public GravityWell gravityWell;
    public float gravityActiveExtraZoom;

    void Start()
    {
        camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void FixedUpdate () {
        Vector3 desiredPosition = transform.position;
        desiredPosition.x = player.transform.position.x;
        desiredPosition.y = player.transform.position.y;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, speed * Time.deltaTime);

        float orthoSize;
        float playerSpeed = player.velocity.magnitude;
        if (playerSpeed < minSpeedZoom)
        {
            orthoSize = minSize;
        } else if (playerSpeed > maxSpeedZoom)
        {
            orthoSize = maxSize;
        } else
        {
            orthoSize = Util.ConvertScale(minSpeedZoom, maxSpeedZoom, minSize, maxSize, playerSpeed);
        }

        if (gravityWell.captureEnabled)
        {
            orthoSize += gravityActiveExtraZoom;
        }

        camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, orthoSize, zoomRate * Time.deltaTime);
	}
}
