using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class PauseMenu : NetworkBehaviour
{
    public string menuScene;
    public GameObject pauseMenu;

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
