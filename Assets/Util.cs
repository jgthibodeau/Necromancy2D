using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Util : MonoBehaviour {
	public static Color orange = new Color(1F, 0.5F, 0F);
	
	public static void DrawRigidbodyRay(Rigidbody rigidBody, Vector3 v1, Vector3 v2){
		Debug.DrawRay (v1 + rigidBody.velocity * Time.fixedDeltaTime, v2);
	}
		
	public static Vector3 RigidBodyPosition(Rigidbody rigidBody){
		return rigidBody.transform.position + rigidBody.velocity * Time.fixedDeltaTime;
    }

    public static void DrawRigidbodyRay(Rigidbody rigidBody, Vector3 start, Vector3 dir, Color color)
    {
        Debug.DrawRay(start + rigidBody.velocity * Time.fixedDeltaTime, dir, color);
    }


    public static void DrawRigidbodyRay(Rigidbody2D rigidBody, Vector2 v1, Vector2 v2)
    {
        Debug.DrawRay(v1 + rigidBody.velocity * Time.fixedDeltaTime, v2);
    }

    public static Vector2 RigidBodyPosition(Rigidbody2D rigidBody)
    {
        return (Vector2)rigidBody.transform.position + rigidBody.velocity * Time.fixedDeltaTime;
    }

    public static void DrawRigidbodyRay(Rigidbody2D rigidBody, Vector2 start, Vector2 dir, Color color)
    {
        Debug.DrawRay(start + rigidBody.velocity * Time.fixedDeltaTime, dir, color);
    }


    public static float SignedVectorAngle(Vector3 referenceVector, Vector3 otherVector, Vector3 normal) {
		Vector3 perpVector;
		float angle;

		//Use the geometry object normal and one of the input vectors to calculate the perpendicular vector
		perpVector = Vector3.Cross(normal, referenceVector);

		//Now calculate the dot product between the perpendicular vector (perpVector) and the other input vector
		angle = Vector3.Angle(referenceVector, otherVector);
		angle *= Mathf.Sign(Vector3.Dot(perpVector, otherVector));

		return angle;
	}

	private IEnumerable WaitForAnimation(Animation animation){
		do {
			yield return null;
		} while(animation.isPlaying);
	}

	public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles) {
		Vector3 direction = point - pivot; //get direction relative to pivot
		direction = Quaternion.Euler (angles) * direction; // rotate
		return direction + pivot; // calculate rotated point
	}

	public static float GetAxis(string axis){
		return TeamUtility.IO.InputManager.GetAxis (axis);
	}
	public static bool GetButton(string button){
		bool pressed = TeamUtility.IO.InputManager.GetButton(button);
        if (!pressed)
        {
            pressed = TeamUtility.IO.InputManager.GetAxis(button) != 0;
        }
        return pressed;
    }
	public static bool GetButtonDown(string button){
		return TeamUtility.IO.InputManager.GetButtonDown (button);
	}
	public static bool GetButtonUp(string button){
		return TeamUtility.IO.InputManager.GetButtonUp (button);
	}

    public static Vector3 MouseInWorld()
    {
        Vector3 mouse = TeamUtility.IO.InputManager.mousePosition;
        mouse = Camera.main.ScreenToWorldPoint(mouse);
        mouse.z = 0;
        return mouse;
    }

    public static bool PolygonContainsPoint(Vector2 point, Vector2[] polygonPoints)
    {
        int j = polygonPoints.Length - 1;
        bool contains = false;
        for (int i = 0; i < polygonPoints.Length; j = i++)
        {
            contains ^= polygonPoints[i].y > point.y ^ polygonPoints[j].y > point.y && point.x < (polygonPoints[j].x - polygonPoints[i].x) * (point.y - polygonPoints[i].y) / (polygonPoints[j].y - polygonPoints[i].y) + polygonPoints[i].x;
        }
        return contains;
    }

    public static bool InLayerMask(int layer, LayerMask layermask) {
		return layermask == (layermask | (1 << layer));
	}

    public static float ConvertScale(float oldMin, float oldMax, float newMin, float newMax, float value)
    {
        value = Mathf.Clamp(value, oldMin, oldMax);
        return (((value - oldMin) * (newMax - newMin)) / (oldMax - oldMin)) + newMin;
    }

    public static bool CanSpawn(Vector3 position, float radius, float height, LayerMask layer)
    {
        Collider[] colliders = Physics.OverlapCapsule(position - Vector3.up * height, position + Vector3.up * height, radius, layer);
        return colliders.Length == 0;
    }

    public static Vector2 RadianToVector2(float radian)
    {
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }

    public static Vector2 DegreeToVector2(float degree)
    {
        return RadianToVector2(degree * Mathf.Deg2Rad);
    }

    public static int GetRandomWeightedIndex(int[] weights)
    {
        // Get the total sum of all the weights.
        int weightSum = 0;
        for (int i = 0; i < weights.Length; ++i)
        {
            weightSum += weights[i];
        }

        // Step through all the possibilities, one by one, checking to see if each one is selected.
        int index = 0;
        int lastIndex = weights.Length - 1;
        while (index < lastIndex)
        {
            // Do a probability check with a likelihood of weights[index] / weightSum.
            if (Random.Range(0, weightSum) < weights[index])
            {
                return index;
            }

            // Remove the last item from the sum of total untested weights and try again.
            weightSum -= weights[index++];
        }

        // No other item was selected, so return very last index.
        return index;
    }
    
    public static UnityEngine.Gradient Lerp(UnityEngine.Gradient a, UnityEngine.Gradient b, float t)
    {
        return Lerp(a, b, t, false, false);
    }

    public static UnityEngine.Gradient LerpNoAlpha(UnityEngine.Gradient a, UnityEngine.Gradient b, float t)
    {
        return Lerp(a, b, t, true, false);
    }

    public static UnityEngine.Gradient LerpNoColor(UnityEngine.Gradient a, UnityEngine.Gradient b, float t)
    {
        return Lerp(a, b, t, false, true);
    }

    static UnityEngine.Gradient Lerp(UnityEngine.Gradient a, UnityEngine.Gradient b, float t, bool noAlpha, bool noColor)
    {
        //list of all the unique key times
        List<float> keysTimes = new List<float>();

        if (!noColor)
        {
            for (int i = 0; i < a.colorKeys.Length; i++)
            {
                float k = a.colorKeys[i].time;
                if (!keysTimes.Contains(k))
                    keysTimes.Add(k);
            }

            for (int i = 0; i < b.colorKeys.Length; i++)
            {
                float k = b.colorKeys[i].time;
                if (!keysTimes.Contains(k))
                    keysTimes.Add(k);
            }
        }

        if (!noAlpha)
        {
            for (int i = 0; i < a.alphaKeys.Length; i++)
            {
                float k = a.alphaKeys[i].time;
                if (!keysTimes.Contains(k))
                    keysTimes.Add(k);
            }

            for (int i = 0; i < b.alphaKeys.Length; i++)
            {
                float k = b.alphaKeys[i].time;
                if (!keysTimes.Contains(k))
                    keysTimes.Add(k);
            }
        }

        GradientColorKey[] clrs = new GradientColorKey[keysTimes.Count];
        GradientAlphaKey[] alphas = new GradientAlphaKey[keysTimes.Count];

        //Pick colors of both gradients at key times and lerp them
        for (int i = 0; i < keysTimes.Count; i++)
        {
            float key = keysTimes[i];
            var clr = Color.Lerp(a.Evaluate(key), b.Evaluate(key), t);
            clrs[i] = new GradientColorKey(clr, key);
            alphas[i] = new GradientAlphaKey(clr.a, key);
        }

        var g = new UnityEngine.Gradient();
        g.SetKeys(clrs, alphas);

        return g;
    }
}