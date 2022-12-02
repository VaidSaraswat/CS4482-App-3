using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public string lobbyScene;
    public TMP_InputField hostNameInputField;
    public TMP_InputField clientNameInputField;

    public void HostGame()
    {
        PlayerPrefs.SetString("PlayerName", hostNameInputField.text.Length == 0 ? "Host" : hostNameInputField.text);
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene(lobbyScene, LoadSceneMode.Single);
    }

    public void JoinGame()
    {
        PlayerPrefs.SetString("PlayerName", clientNameInputField.text.Length == 0 ? "Default User" : clientNameInputField.text);
        NetworkManager.Singleton.StartClient();
    }    

    public void ExitGame()
    {
        Application.Quit();
    }
}
