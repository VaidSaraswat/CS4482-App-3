using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public string lobbyScene;
    public TMP_InputField hostNameInputField;
    public TMP_InputField clientNameInputField;
    public TMP_InputField hostIPAddressInputField;

    public void HostGame()
    {
        PlayerPrefs.SetString("PlayerName", hostNameInputField.text.Length == 0 ? "Host" : hostNameInputField.text);
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.SetClientSynchronizationMode(LoadSceneMode.Single);
        NetworkManager.Singleton.SceneManager.LoadScene(lobbyScene, LoadSceneMode.Single);
    }

    public void JoinGame()
    {
        PlayerPrefs.SetString("PlayerName", clientNameInputField.text.Length == 0 ? "Default User" : clientNameInputField.text);
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = hostIPAddressInputField.text;
        NetworkManager.Singleton.StartClient();
        NetworkManager.Singleton.SceneManager.SetClientSynchronizationMode(LoadSceneMode.Single);
        StartCoroutine(waiter());
    }    

    IEnumerator waiter()
    {
        yield return new WaitForSeconds(0.5f);
        if(NetworkManager.Singleton.IsConnectedClient == false)
        {
            NetworkManager.Singleton.Shutdown();
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
