using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FadeAudio))]
public class GravityWell : MonoBehaviour {
    public FadeAudio fadeAudio;

    public List<Capturable> capturedObjects = new List<Capturable>();
    public int capturedLayer = 13;
    
    public GameObject lineRendererPrefab;
    public Dictionary<Capturable, GameObject> lineRenderers = new Dictionary<Capturable, GameObject>();

    public Vector3 previousPosition;
    public float movementForce;

    public bool addForceWhenMoving;
    public bool alwaysAdjustGravity;
    public bool captureEnabled;
    public bool reverse;

    public Transform gravityWellBase;
    public Quaternion currentRotation;
    public float rotateSpeed;

    public GameObject captureEffectEnabled;
    public GameObject captureEffectReversed;
    public GameObject captureEffectDisabled;
    
    public float maxDistance = 3;
    public float gravityForce = 20;

    public bool useGravityRange;
    public float minGravityForce, maxGravityForce;
    public float minGravityReverseForce, maxGravityReverseForce;
    public float minGravityDistance, maxGravityDistance;
    public float minGravityReverseDistance, maxGravityReverseDistance;

    public Transform gravitySpot;
    public Transform reverseGravitySpot;

    bool previouslyReversed;

    // Use this for initialization
    void Start () {
        fadeAudio = GetComponent<FadeAudio>();
        currentRotation = gravityWellBase.transform.rotation;
        previousPosition = transform.position;
    }

    // Update is called once per frame
	void Update ()
    {
        reverse = Util.GetButton("Thrust");
        captureEnabled = reverse || Util.GetButton("Gravity");

        fadeAudio.enabled = captureEnabled;

        if (alwaysAdjustGravity || !captureEnabled)
        {
            AdjustGravityWell();
        }


        captureEffectEnabled.SetActive(captureEnabled && !reverse);
        captureEffectReversed.SetActive(captureEnabled && reverse);
        captureEffectDisabled.SetActive(!captureEnabled);

        if (previouslyReversed && !reverse)
        {
            ReleaseAll();
        }

        CleanNullCapturedObjects();

        if (useGravityRange)
        {
            if (reverse)
            {
                DebugExtension.DebugCircle(transform.position, Vector3.forward, Color.white, minGravityReverseDistance);
                DebugExtension.DebugCircle(transform.position, Vector3.forward, Color.white, maxGravityReverseDistance);
            }
            else
            {
                DebugExtension.DebugCircle(transform.position, Vector3.forward, Color.white, minGravityDistance);
                DebugExtension.DebugCircle(transform.position, Vector3.forward, Color.white, maxGravityDistance);
            }
        } else
        {
            DebugExtension.DebugCircle(transform.position, Vector3.forward, Color.red, maxDistance);
        }

        if (captureEnabled)
        {
            List<Capturable> capturablesToRemove = new List<Capturable>();

            foreach (Capturable capturable in capturedObjects)
            {
                Kill kill = capturable.GetComponent<Kill>();
                if (kill != null)
                {
                    kill.Reset();
                }

                //if (capturable.rotateWithGravity)
                //{
                //    capturable.transform.rotation = Quaternion.Lerp(capturable.transform.rotation, currentRotation, rotateSpeed * Time.deltaTime);
                //}

                if (useGravityRange)
                {
                    ApplyGravityV2(capturable, capturablesToRemove);
                }
                else
                {
                    ApplyGravityV1(capturable, capturablesToRemove);
                }
            }

            foreach (Capturable capturable in capturablesToRemove)
            {
                Release(capturable);
            }
        } else
        {
            ReleaseAll();
        }

        previousPosition = transform.position;
        previouslyReversed = reverse;
    }

    void CleanNullCapturedObjects() {
        capturedObjects.RemoveAll(item => item == null);
    }

    void ApplyGravityV1(Capturable capturable, List<Capturable> capturablesToRemove)
    {
        Vector3 force = Vector3.zero;
        float distance = Vector3.Distance(capturable.transform.position, transform.position);
        //if (distance < maxDistance)
        //{
        //} else
        //{
        //capturablesToRemove.Add(capturable);
        //}
        if (reverse)
        {
            force = (capturable.transform.position - reverseGravitySpot.transform.position) * CalculateGravityForce(distance, capturable);
            if (distance > maxDistance)
            {
                capturablesToRemove.Add(capturable);
            }
        } else
        {
            force = (gravitySpot.transform.position - capturable.transform.position) * CalculateGravityForce(distance, capturable);
        }
        capturable.GetRigidbody().AddForce(force);
        Debug.DrawRay(capturable.transform.position, force, Color.green);

        if (addForceWhenMoving)
        {
            force = (transform.position - previousPosition) * movementForce;

            capturable.GetRigidbody().AddForce(force);
            Debug.DrawRay(capturable.transform.position, force, Color.red);
        }
    }

    void ApplyGravityV2(Capturable capturable, List<Capturable> capturablesToRemove)
    {
        Vector3 force = Vector3.zero;
        float distance = Vector3.Distance(capturable.transform.position, transform.position);
        if (distance < maxGravityDistance)
        {
            if (reverse)
            {
                force = (capturable.transform.position - reverseGravitySpot.transform.position) * CalculateGravityForceV2(distance, capturable);
                if (distance > maxGravityReverseDistance)
                {
                    capturablesToRemove.Add(capturable);
                }
            }
            else
            {
                force = (gravitySpot.transform.position - capturable.transform.position) * CalculateGravityForceV2(distance, capturable);
            }
            capturable.GetRigidbody().AddForce(force);
            Debug.DrawRay(capturable.transform.position, force, Color.green);
        }
        else
        {
            capturablesToRemove.Add(capturable);
        }

        if (addForceWhenMoving)
        {
            force = (transform.position - previousPosition) * movementForce;

            capturable.GetRigidbody().AddForce(force);
            Debug.DrawRay(capturable.transform.position, force, Color.red);
        }
    }

    float CalculateGravityForce(float distance, Capturable capturable)
    {
        //Gm1m2 / r2
        //return gravityForce;
        if (distance > 0)
        {
            return (gravityForce) / (distance);
        }
        else
        {
            return 0;
        }
    }

    float CalculateGravityForceV2(float distance, Capturable capturable)
    {
        if (reverse)
        {
            //return Util.ConvertScale(minGravityReverseDistance, maxGravityReverseDistance, minGravityReverseForce, maxGravityReverseForce, distance);
            return Util.ConvertScale(minGravityReverseDistance, maxGravityReverseDistance, maxGravityReverseForce, minGravityReverseForce, distance);
        }
        else
        {
            return Util.ConvertScale(minGravityDistance, maxGravityDistance, minGravityForce, maxGravityForce, distance);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (captureEnabled)
        {
            Capturable capturable = other.gameObject.GetComponentInParent<Capturable>();
            Capture(capturable);
        }
    }

    public void Capture(Capturable capturable)
    {
        if (CanCapture(capturable))
        {
            Physics2D.IgnoreCollision(capturable.GetComponent<Collider2D>(), GetComponent<Collider2D>(), true);
            capturedObjects.Add(capturable);
            capturable.SetCaptured(true, capturedLayer, this);

            GameObject line = GameObject.Instantiate(lineRendererPrefab);
            LineBetweenObjects lbo = line.GetComponent<LineBetweenObjects>();
            lbo.go1 = gameObject;
            lbo.go2 = capturable.gameObject;
            lineRenderers.Add(capturable, line);
        }
    }

    public bool CanCapture(Capturable capturable)
    {
        return capturable != null && !capturedObjects.Contains(capturable);
    }

    public void Release(Capturable capturable)
    {
        if (capturedObjects.Contains(capturable))
        {
            capturedObjects.Remove(capturable);
            capturable.SetCaptured(false, capturedLayer, this);

            Physics2D.IgnoreCollision(capturable.GetComponent<Collider2D>(), GetComponent<Collider2D>(), false);
            GameObject line;
            lineRenderers.TryGetValue(capturable, out line);
            lineRenderers.Remove(capturable);
            GameObject.Destroy(line);
        }
    }

    public void ReleaseAll()
    {
        foreach (Capturable capturable in capturedObjects)
        {
            capturable.SetCaptured(false, capturedLayer, this);

            Physics2D.IgnoreCollision(capturable.GetComponent<Collider2D>(), GetComponent<Collider2D>(), false);
            GameObject line;
            lineRenderers.TryGetValue(capturable, out line);
            lineRenderers.Remove(capturable);
            GameObject.Destroy(line);
        }
        capturedObjects.Clear();
    }

    void AdjustGravityWell()
    {
        Vector2 input = new Vector2(Util.GetAxis("Horizontal Right"), Util.GetAxis("Vertical Right"));
        if (input.magnitude > 0)
        {
            //float angle = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg;
            //Quaternion desiredRotation = Quaternion.Euler(new Vector3(0, angle, 0));
            float angle = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg;
            Quaternion desiredRotation = Quaternion.Euler(new Vector3(0, 0, -angle));

            currentRotation = Quaternion.Lerp(currentRotation, desiredRotation, rotateSpeed * Time.deltaTime);
        }

        gravityWellBase.transform.rotation = currentRotation;
    }
}
