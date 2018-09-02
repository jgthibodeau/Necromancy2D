using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    private ShowPanels showPanels;
    public static MenuController instance = null;
    public Menu gameOverMenu;
    public Text scoreText;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            showPanels = GetComponentInChildren<ShowPanels>();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void GameOver(string score)
    {
        Debug.Log("GAME OVER");
        scoreText.text = "FINAL SCORE:\n" + score;
        showPanels.Show(gameOverMenu);
    }
}
