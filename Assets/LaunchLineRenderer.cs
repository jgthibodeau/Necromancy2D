using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaunchLineRenderer : MonoBehaviour {
    public LineRenderer lineRenderer;
    public Vector3 point1, point2;
    private bool hasPoints;

    [Tooltip("The percent of the line that is consumed by the arrowhead")]
    [Range(0, 100)]
    public float PercentHead = 0.4f;
    public float arrowWidth = 2f;
    public float mainWidth = 1f;

    // Use this for initialization
    void Start () {
        lineRenderer = GetComponent<LineRenderer>();
	}

    void Update()
    {
        if (hasPoints)
        {
            //lineRenderer.positionCount = 2;
            //lineRenderer.SetPositions(new Vector3[] { point1, point2 });
            UpdateArrow(point1, point2);
        }
        else
        {
            Reset();
        }
    }
	
    public void Set(Vector3 point1, Vector3 point2, Gradient gradient)
    {
        this.point1 = point1;
        this.point2 = point2;

        hasPoints = (point1 != null && point2 != null);

        lineRenderer.colorGradient = gradient;
        UpdateArrow(point1, point2);
    }
    
    void UpdateArrow(Vector3 point1, Vector3 point2)
    {
        float AdaptiveSize = (float)(PercentHead / Vector3.Distance(point1, point2));

        if (lineRenderer == null)
        {
            lineRenderer = this.GetComponent<LineRenderer>();
        }

        lineRenderer.widthCurve = new AnimationCurve(
            new Keyframe(0, mainWidth)
            , new Keyframe(0.999f - AdaptiveSize, mainWidth)  // neck of arrow
            , new Keyframe(1 - AdaptiveSize, arrowWidth)  // max width of arrow head
            , new Keyframe(1, 0f, -5, -5));  // tip of arrow

        lineRenderer.positionCount = 4;
        lineRenderer.SetPositions(new Vector3[] {
               point1
               , Vector3.Lerp(point1, point2, 0.999f - AdaptiveSize)
               , Vector3.Lerp(point1, point2, 1 - AdaptiveSize)
               , point2 });
    }


    public void Reset()
    {
        hasPoints = false;
        lineRenderer.positionCount = 0;
        lineRenderer.SetPositions(new Vector3[0]);
    }
}
