using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stamina : MonoBehaviour {
    public float maxStamina;
    public float currentStamina;
    public float staminaUseRate;
    public float staminaRegainRate;
    public float staminaRegainDelay;
    public float staminaRegainDelayDepleted;

    public bool showBarWhenFull;
    public GameObject staminaBar;
    public Image staminaBarFill;

    private float currentStaminaRegainDelay;
    
	void Start () {
        currentStamina = maxStamina;
	}

    // Update is called once per frame
    void Update()
    {
        if (currentStaminaRegainDelay > 0)
        {
            currentStaminaRegainDelay -= Time.deltaTime;
        }
        else if (currentStamina < maxStamina)
        {
            currentStamina = Mathf.Min(maxStamina, currentStamina + staminaRegainRate * Time.deltaTime);
        }

        if (currentStamina < maxStamina || showBarWhenFull)
        {
            staminaBar.SetActive(true);
            staminaBarFill.fillAmount = currentStamina / maxStamina;
        } else
        {
            staminaBar.SetActive(false);
        }
    }

    public bool HasStamina()
    {
        return currentStamina > 0;
    }

    public bool FullStamina()
    {
        return currentStamina >= maxStamina;
    }

    public bool UseStamina()
    {
        if (!HasStamina())
        {
            return false;
        }

        currentStamina -= staminaUseRate;

        if (currentStamina <= 0)
        {
            currentStamina = 0;
            currentStaminaRegainDelay = staminaRegainDelayDepleted;
        } else
        {
            currentStaminaRegainDelay = staminaRegainDelay;
        }

        return true;
    }
}
