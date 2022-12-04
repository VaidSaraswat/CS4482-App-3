using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public GameObject PlayerPrefab;
    public GameObject VirtualCamera;

    //Inventory Variables
    public GameObject slotHolder;
    public GameObject shop;
    public TMP_Text prompt;
    public TMP_Text pointDisplay;

    //Scene Change Variables
    private GameObject[] allObjects;
    private bool [] states;
    private bool [] canvasStates;
    private GameObject camera;
    private GameObject canvas;
    private GameObject player;

    private Dictionary<ulong, PlayerData> m_PlayerData = new Dictionary<ulong, PlayerData>();
    private Dictionary<ulong, GameObject> m_Players = new Dictionary<ulong, GameObject>();

    void Start()
    {
        camera = GameObject.Find("Main Camera");
        canvas = GameObject.Find("Canvas");
        
        m_PlayerData[NetworkManager.Singleton.LocalClientId] = new PlayerData(PlayerPrefs.GetString("PlayerName", "Default User"), 0);
        SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    void Update()
    {
        //m_PlayerData[NetworkManager.Singleton.LocalClientId].Score = ...
        UpdatePlayerDataServerRpc(NetworkManager.Singleton.LocalClientId, m_PlayerData[NetworkManager.Singleton.LocalClientId]);
    }

    public void sendToCombat(){
        player = GameObject.FindWithTag("Player");
        camera.SetActive(false);
        player.SetActive(false);

        canvasStates = new bool[canvas.transform.childCount];
        for(int i = 0; i < canvas.transform.childCount; i++){
            canvasStates[i] = canvas.transform.GetChild(i).gameObject.activeSelf;
            canvas.transform.GetChild(i).gameObject.SetActive(false);
        }

        allObjects = GameObject.FindGameObjectsWithTag("main");
        states = new bool[allObjects.Length];
        for(int i =0; i<allObjects.Length; i++)
        {
            if(allObjects[i].name != "GameManager")
            {
            states[i] =allObjects[i].activeSelf;
            allObjects[i].gameObject.SetActive(false); 
            }
        }   
        SceneManager.LoadScene("FightScene", LoadSceneMode.Additive);
    }

    public void returnfromCombat(bool win){
        SceneManager.UnloadSceneAsync("FightScene");
        
        for(int i =0; i<allObjects.Length; i++)
        {
            if(allObjects[i] != null){
                if(allObjects[i].name != "GameManager"){
                    allObjects[i].SetActive(states[i]);
                }  
            }
        }
        for(int i =0; i<canvasStates.Length; i++)
        {
            canvas.transform.GetChild(i).gameObject.SetActive(canvasStates[i]);
        }
        player.SetActive(true);
        camera.SetActive(true); 

        if(win){
            player.GetComponent<Inventory>().addPoints(50);
        }
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
        SetupLocalPlayerClientRpc(new ClientRpcParams {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { clientId }
                }
            });
    }

    [ClientRpc]
    public void SetupLocalPlayerClientRpc(ClientRpcParams clientRpcParams) {
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