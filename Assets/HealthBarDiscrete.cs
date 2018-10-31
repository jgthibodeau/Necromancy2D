using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarDiscrete : MonoBehaviour {
    public int healthPerBar = 1;
    public GameObject icon;
    public Health health;
    public float offset;

    public List<Image> images;

    private float currentHealth;

    // Use this for initialization
    void Start ()
    {
        //images.Add(GetIconFromObject(icon));

        //int numberIcons = Mathf.CeilToInt(health.maxHealth / healthPerBar) - 1;

        //for(int i=0; i<numberIcons; i++)
        //{
        //    GameObject newIcon = GameObject.Instantiate(icon, transform);
        //    RectTransform newRect = newIcon.GetComponent<RectTransform>();
        //    Vector3 position = newRect.position;
        //    position.x += offset * (i + 1);
        //    newRect.position = position;

        //    images.Add(GetIconFromObject(newIcon));
        //}
	}

    // Update is called once per frame
    void Update()
    {
        float healthRemaining = health.currentHealth;
        if (health.IsDead())
        {
            healthRemaining = -1f;
        }

        if (healthRemaining != currentHealth)
        {
            foreach (Image i in images)
            {
                healthRemaining = FillImage(i, healthRemaining);
            }
        }
    }

    private float FillImage(Image image, float healthRemaining)
    {
        if (healthRemaining >= healthPerBar)
        {
            image.fillAmount = 1;
            healthRemaining -= healthPerBar;
        }
        else if (healthRemaining > 0)
        {
            image.fillAmount = healthRemaining / healthPerBar;
            healthRemaining = 0;
        }
        else
        {
            image.fillAmount = 0;
        }

        return healthRemaining;
    }

    private Image GetIconFromObject(GameObject go)
    {
        foreach (Image i in go.GetComponentsInChildren<Image>())
        {
            if (i.tag == "UI Fill")
            {
                return i;
            }
        }
        return null;
    }
}
