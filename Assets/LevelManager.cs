using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance = null;
    public static MenuController menuController;
    public int currentScore;
    public Text scoreText;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            currentScore = 0;
            menuController = MenuController.instance;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        UpdateScoreText();
    }

    public void AddScore(int score)
    {
        currentScore += score;
    }

    void UpdateScoreText()
    {
        scoreText.text = string.Format("{0:n0}", currentScore);
    }

    private bool playerDead = false;
    public void OnPlayerDeath()
    {
        if (!playerDead)
        {
            playerDead = true;
            menuController.GameOver(scoreText.text);
        }
    }

    public bool IsPlayerDead()
    {
        return playerDead;
    }
}
