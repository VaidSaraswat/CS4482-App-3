using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using TMPro;

public class PauseMenu : NetworkBehaviour
{
    public string menuScene;
    public GameObject pauseMenu;
    public GameObject scoreMenu;
    public TMP_Text scores;

    private NetworkVariable<bool> isPausedByAnyPlayer = new NetworkVariable<bool>(false);
    private bool wasPausedByAnyPlayer = false;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        if(!NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += ServerTerminate;
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGameServerRpc(!isPausedByAnyPlayer.Value);
        }

        if(isPausedByAnyPlayer.Value != wasPausedByAnyPlayer)
        {
            pauseMenu.SetActive(isPausedByAnyPlayer.Value);
            Time.timeScale = isPausedByAnyPlayer.Value ? 0f : 1f;
            Cursor.lockState = isPausedByAnyPlayer.Value ? CursorLockMode.None : CursorLockMode.Locked;
            wasPausedByAnyPlayer = isPausedByAnyPlayer.Value;
        }
        
        if(Input.GetKey(KeyCode.Tab) && isPausedByAnyPlayer.Value == false)
        {
            if(scoreMenu.activeSelf == false)
            {
                scoreMenu.SetActive(true);
            }
            MovementManager[] players = GameObject.FindObjectsOfType<MovementManager>();
            Array.Sort(players, CompareScores);
            string[] lines = players.Select(i => i.playerName.Text + ": " + i.score.ToString()).ToArray();
            scores.text = lines.Aggregate("", (current, next) => current + "\n" + next).Substring(1);
        }
        else if(scoreMenu.activeSelf)
        {
            scoreMenu.SetActive(false);
        }
    }

    public override void OnDestroy()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        if(!NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= ServerTerminate;
        }
    }

    private static int CompareScores(MovementManager p1, MovementManager p2)
    {
        if(p1.score > p2.score)
        {
            return -1;
        }
        else if(p1.score < p2.score)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    public void ReturnToMainMenu()
    {
        if(NetworkManager.Singleton.IsServer)
        {
            for (int i = 0; i < NetworkManager.Singleton.ConnectedClientsList.Count; i++)
            {
                ulong id = NetworkManager.Singleton.ConnectedClientsList[i].ClientId;
                if (NetworkManager.Singleton.LocalClientId == id) continue;        
                NetworkManager.Singleton.DisconnectClient(id);
            }
        }
        
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene(menuScene);
    }

    private void ServerTerminate(ulong clientId)
    {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene(menuScene);
    }

    [ServerRpc(RequireOwnership = false)]
    public void PauseGameServerRpc(bool pauseState)
    {
        isPausedByAnyPlayer.Value = pauseState;
    }
}
