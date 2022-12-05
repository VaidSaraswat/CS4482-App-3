using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using TMPro;

public class Timer : NetworkBehaviour
{
    public GameObject ScoreMenu;
    public TMP_Text timerText;
    public TMP_Text gameOverText;
    public TMP_Text scoreText;

    private float timeLeft = 300f;
    private bool timerOn = true;

    void Update()
    {
        if(NetworkManager.Singleton.IsServer && timeLeft > 0 && GetComponent<PauseMenu>().isPausedByAnyPlayer.Value == false)
        {
            timeLeft -= Time.deltaTime;
            UpdateTimeClientRpc(timeLeft);
        }
        else if(timeLeft < 0 && timerOn)
        {
            timerOn = false;
            EndGame();
        }

        UpdateTimer(timeLeft);
    }

    [ClientRpc]
    void UpdateTimeClientRpc(float newTimeLeft)
    {
        if(!NetworkManager.Singleton.IsServer)
        {
            timeLeft = newTimeLeft;
        }
    }

    private async void EndGame()
    {
        Destroy(timerText);
        GetComponent<PauseMenu>().updateEnabled = false;
        Time.timeScale = 0f;

        ScoreMenu.SetActive(true);
        ScoreMenu.gameObject.SetActive(true);
        gameOverText.text = "Match Results";
        scoreText.text = GetComponent<PauseMenu>().GetScoresText();
        
        await Task.Delay(5000);
        GetComponent<PauseMenu>().ReturnToMainMenu();
    }

    void UpdateTimer(float currentTime)
    {
        currentTime += 1;
        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);
        timerText.text = string.Format("Time Left: {0:00} : {1:00}", minutes, seconds);

    }
}