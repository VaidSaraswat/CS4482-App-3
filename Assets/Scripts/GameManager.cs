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
    public GameObject CurrentPlayer;

    //Inventory Variables
    public GameObject slotHolder;
    public GameObject shop;
    public TMP_Text prompt;
    public TMP_Text pointDisplay;

    //Scene Change Variables
    private GameObject[] allObjects;
    private GameObject[] player;
    private bool [] states;
    private bool [] canvasStates;
    private GameObject camera;
    private GameObject canvas;
    

    private Dictionary<ulong, GameObject> m_Players = new Dictionary<ulong, GameObject>();

    void Awake()
    {
        if(NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += RemovePlayer;
        }
    }

    void Start()
    {
        camera = GameObject.Find("Main Camera");
        canvas = GameObject.Find("Canvas");

        SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId, PlayerPrefs.GetString("PlayerName", "Default User"));
    }

    public override void OnDestroy()
    {
        if(NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= RemovePlayer;
        }
    }

    private void RemovePlayer(ulong clientId)
    {
        Destroy(m_Players[clientId]);
        m_Players.Remove(clientId);
        UpdateMovementManagerPlayerIdsServerRpc();
    }

    public void sendToCombat(){
        player = GameObject.FindGameObjectsWithTag("Player");
        camera.SetActive(false);
        
        for(int i =0;i<player.Length;i++){
            player[i].SetActive(false);
        }

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

        for(int i =0;i<player.Length;i++){
            player[i].SetActive(false);
        }
        camera.SetActive(true); 

        if(win){
            this.GetPlayer().GetComponent<Inventory>().addPoints(50);
        }
    }

    public GameObject GetPlayer()
    {
        if(CurrentPlayer != null)
        {
            return CurrentPlayer;
        }
        return null;
    }

    [ServerRpc(RequireOwnership=false)]
    public void SpawnPlayerServerRpc(ulong clientId, string name) {
        m_Players[clientId] = Instantiate(PlayerPrefab);
        m_Players[clientId].GetComponent<MovementManager>().playerId.Value = clientId;
        m_Players[clientId].GetComponent<MovementManager>().playerName.Text = name;
        m_Players[clientId].GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
        SetupLocalPlayerClientRpc(new NetworkObjectReference(m_Players[clientId].GetComponent<NetworkObject>()), new ClientRpcParams {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { clientId }
                }
            });
        UpdateMovementManagerPlayerIdsServerRpc();
    }

    [ClientRpc]
    public void SetupLocalPlayerClientRpc(NetworkObjectReference currentPlayerRef, ClientRpcParams clientRpcParams) {
        StartCoroutine(SetupLocalPlayerCoroutine(currentPlayerRef));
    }

    IEnumerator SetupLocalPlayerCoroutine(NetworkObjectReference currentPlayerRef)
    {
        while(currentPlayerRef.TryGet(out NetworkObject i) == false)
        {
            yield return new WaitForSeconds(0.1f);
        }
        
        currentPlayerRef.TryGet(out NetworkObject currentPlayerNetworkObject);
        CurrentPlayer = currentPlayerNetworkObject.gameObject;
        VirtualCamera.GetComponent<CinemachineVirtualCamera>().Follow = CurrentPlayer.transform.GetChild(0).transform;
        CurrentPlayer.GetComponent<Inventory>().slotHolder = slotHolder;
        CurrentPlayer.GetComponent<Inventory>().shop = shop;
        CurrentPlayer.GetComponent<Inventory>().prompt = prompt;
        CurrentPlayer.GetComponent<Inventory>().pointDisplay = pointDisplay;
    }

    [ServerRpc]
    void UpdateMovementManagerPlayerIdsServerRpc()
    {
        foreach(ulong playerId in m_Players.Keys)
        {
            ulong[] list = RemovePlayerId(playerId);
            m_Players[playerId].GetComponent<MovementManager>().playerIds.Clear();
            foreach(ulong id in list)
            {
                m_Players[playerId].GetComponent<MovementManager>().playerIds.Add(id);
            }
        }
    }

    private ulong[] RemovePlayerId(ulong playerId)
    {
        return m_Players.Keys.Where(x => x != playerId).ToArray();
    }
}