using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class GameManager : NetworkBehaviour
{
    public GameObject PlayerPrefab;
    public GameObject VirtualCamera;

    //Inventory Variables
    public GameObject slotHolder;
    public GameObject shop;
    public TMP_Text prompt;
    public TMP_Text pointDisplay;

    private Dictionary<ulong, PlayerData> m_PlayerData = new Dictionary<ulong, PlayerData>();
    private Dictionary<ulong, GameObject> m_Players = new Dictionary<ulong, GameObject>();

    void Start()
    {
        m_PlayerData[NetworkManager.Singleton.LocalClientId] = new PlayerData(PlayerPrefs.GetString("PlayerName", "Default User"), 0);
        SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    void Update()
    {
        //m_PlayerData[NetworkManager.Singleton.LocalClientId].Score = ...
        UpdatePlayerDataServerRpc(NetworkManager.Singleton.LocalClientId, m_PlayerData[NetworkManager.Singleton.LocalClientId]);
    }

    public GameObject GetPlayer()
    {
        return NetworkManager.LocalClient.PlayerObject.gameObject;
    }

    [ServerRpc(RequireOwnership=false)]
    public void SpawnPlayerServerRpc(ulong clientId) {
        m_Players[clientId] = Instantiate(PlayerPrefab);
        m_Players[clientId].GetComponent<MovementManager>().playerId.Value = clientId;
        m_Players[clientId].GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
        SetupCameraClientRpc(new ClientRpcParams {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { clientId }
                }
            });
    }

    [ClientRpc]
    public void SetupCameraClientRpc(ClientRpcParams clientRpcParams) {
        GameObject player = NetworkManager.LocalClient.PlayerObject.gameObject;
        VirtualCamera.GetComponent<CinemachineVirtualCamera>().Follow = player.transform.GetChild(0).transform;
        player.GetComponent<Inventory>().slotHolder = slotHolder;
        player.GetComponent<Inventory>().shop = shop;
        player.GetComponent<Inventory>().prompt = prompt;
        player.GetComponent<Inventory>().pointDisplay = pointDisplay;
    }

    [ServerRpc(RequireOwnership=false)]
    public void UpdatePlayerDataServerRpc(ulong clientId, PlayerData data)
    {
        UpdatePlayerDataClientRpc(clientId, data);
    }

    [ClientRpc]
    public void UpdatePlayerDataClientRpc(ulong clientId, PlayerData data)
    {
        if(clientId != NetworkManager.Singleton.LocalClientId)
        {
            m_PlayerData[clientId] = data;
        }
    }
}

public struct PlayerData : INetworkSerializeByMemcpy
{
    public PlayerData(string name, int score)
    {
        Name = name;
        Score = score;
    }

    public string Name;
    public int Score;
}