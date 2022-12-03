using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    public TMP_Text timerText;
    public TMP_Text gameOverText;
    private float timeLeft = 300;
    private bool timerOn = false;
    void Start()
    {
        timerOn = true;
        gameOverText.gameObject.SetActive(false);
    }


    // Update is called once per frame
    async void Update()
    {
        if (timerOn)
        {
            if (timeLeft > 0)
            {
                timeLeft -= Time.deltaTime;
                updateTimer(timeLeft);
            }
            else
            {
                timeLeft = 0;
                timerOn = false;
                gameOverText.gameObject.SetActive(true);
                await Task.Delay(3000);
                SceneManager.LoadScene("MainMenu");

            }
        }
    }


    void updateTimer(float currentTime)
    {
        currentTime += 1;
        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);
        timerText.text = string.Format("Time Left: {0:00} : {1:00}", minutes, seconds);

    }
}

