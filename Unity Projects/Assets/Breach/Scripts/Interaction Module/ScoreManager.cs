using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    
   
    private int Score;

    //counting down time 
    [SerializeField] private float gameOverTime = 120f;
    public bool TimerOn = false;
    [SerializeField] private TMP_Text gameOverTimeText;
    private float Timer = 0f;
    private float timeLeft;


    void Start()
    {
        Score = 0; 
    }

    // Update is called once per frame
    void Update()
    {
        if (TimerOn)
        {
            if (Timer <= gameOverTime)
            {
                Timer += Time.deltaTime;
                timeLeft = gameOverTime - Timer;
                updateTimer(timeLeft);
            }
            else
            {
                gameOverTimeText.text = "GameOver!";
            }

        }
    }
        public void UpdateScore(int pontsToAdd)
    {
        Score += pontsToAdd;
        scoreText.text = "Score: " + Score.ToString(); 
    }

    void updateTimer(float myTime)
    {
        myTime += 1;
        float minutes = Mathf.FloorToInt(myTime / 60);
        float seconds = Mathf.FloorToInt(myTime % 60);
        gameOverTimeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
