using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using TMPro;

public class LobbyMenu : NetworkBehaviour
{
    public string menuScene;
    public string gameScene;
    public GameObject ServerOverlay;
    public GameObject ClientOverlay;
    public TMP_Text PlayersText;
    public TMP_Text PlayerCountText;

    private Dictionary<ulong, string> m_PlayerNames = new Dictionary<ulong, string>(); 
    private NetworkVariable<int> m_PlayerCount = new NetworkVariable<int>();

    void Start()
    {
        if(NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += RemovePlayerName;
            ServerOverlay.SetActive(true);
            SetPlayerNameServerRpc(NetworkManager.Singleton.LocalClientId, PlayerPrefs.GetString("PlayerName", "Host"));
        }
        else
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += ServerTerminate;
            ClientOverlay.SetActive(true);
            SetPlayerNameServerRpc(NetworkManager.Singleton.LocalClientId, PlayerPrefs.GetString("PlayerName", "Default User"));
        }
    }

    void Update()
    {
        if(NetworkManager.Singleton.IsServer)
        {
            m_PlayerCount.Value = NetworkManager.Singleton.ConnectedClientsList.Count;
        }
        PlayerCountText.text = "Players: " + m_PlayerCount.Value;
    }
    
    public void LeaveLobby()
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

    public void StartGame()
    {
        NetworkManager.Singleton.SceneManager.LoadScene(gameScene, LoadSceneMode.Single);
    }

    private void ServerTerminate(ulong clientId)
    {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene(menuScene);
    }

    private void RemovePlayerName(ulong clientId)
    {
        m_PlayerNames.Remove(clientId);
        var playerNames = m_PlayerNames.Select(x => x.Value).ToArray().Aggregate("", (current, next) => current + "\n" + next);
        UpdatePlayerNamesClientRpc(playerNames);
    }

    [ClientRpc]
    void UpdatePlayerNamesClientRpc(string playerNames)
    {
        PlayersText.text = playerNames;
    }

    [ServerRpc(RequireOwnership = false)]
    void SetPlayerNameServerRpc(ulong id, string name)
    {
        m_PlayerNames[id] = name;
        var playerNames = m_PlayerNames.Select(x => x.Value).ToArray().Aggregate("", (current, next) => current + "\n" + next);
        UpdatePlayerNamesClientRpc(playerNames);
    }
}