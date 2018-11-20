using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SummonCount : MonoBehaviour
{
    public TextMeshProUGUI text;
    public int maxSummons;
    public int currentSummons;

    public float normalScale, maxScale, minScale;
    public float scaleIncreaseSpeed, scaleDecreaseSpeed;

    private Coroutine scaleCoroutine;

    public void SetSummonCount(int count)
    {
        if (currentSummons != count)
        {
            if (scaleCoroutine == null)
            {
                if (count > currentSummons)
                {
                    scaleCoroutine = StartCoroutine(ScaleText());
                } else
                {
                    scaleCoroutine = StartCoroutine(ScaleTextDown());
                }
            }

            currentSummons = count;
            string summonText = currentSummons + " / " + maxSummons;
            text.SetText(summonText);
        }
    }

    private IEnumerator ScaleText()
    {
        //increase
        while (text.fontSize < maxScale)
        {
            text.fontSize += scaleIncreaseSpeed * Time.deltaTime;
            if (text.fontSize > maxScale)
            {
                text.fontSize = maxScale;
                break;
            }
            yield return null;
        }
        //decrease
        while (text.fontSize > normalScale)
        {
            text.fontSize -= scaleDecreaseSpeed * Time.deltaTime;
            if (text.fontSize < normalScale)
            {
                text.fontSize = normalScale;
                break;
            }
            yield return null;
        }
        //stop
        scaleCoroutine = null;
    }

    private IEnumerator ScaleTextDown()
    {
        //increase
        while (text.fontSize > minScale)
        {
            text.fontSize -= scaleIncreaseSpeed * Time.deltaTime;
            if (text.fontSize < minScale)
            {
                text.fontSize = minScale;
                break;
            }
            yield return null;
        }
        //decrease
        while (text.fontSize < normalScale)
        {
            text.fontSize += scaleDecreaseSpeed * Time.deltaTime;
            if (text.fontSize > normalScale)
            {
                text.fontSize = normalScale;
                break;
            }
            yield return null;
        }
        //stop
        scaleCoroutine = null;
    }
}
