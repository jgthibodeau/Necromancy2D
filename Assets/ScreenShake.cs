﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    MyGameManager myGameManager;
    public static ScreenShake instance = null;

    public float shakeDuration = 0f;
    public float shakeAmount = 0f;
    public float maxShakeAmount = 2f;
    public float decreaseFactor = 1.0f;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void Shake(float duration, float amount)
    {
        if (duration > shakeDuration)
        {
            shakeDuration = duration;
        }

        if (amount > shakeAmount)
        {
            shakeAmount = amount;
        }
        //shakeAmount = Mathf.Clamp(shakeAmount + amount, 0, maxShakeAmount);
    }

    void Update() {
        if (shakeDuration > 0 && !MyGameManager.instance.isPaused)
        {
            //Debug.Log("Shaking " + shakeAmount);
            transform.localPosition = transform.localPosition + (Vector3)Random.insideUnitCircle * shakeAmount;
            shakeDuration -= Time.deltaTime * decreaseFactor;
        } else
        {
            shakeAmount = 0;
        }
    }
}
