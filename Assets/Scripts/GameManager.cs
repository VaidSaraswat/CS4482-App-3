using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public GameObject PlayerPrefab;
    public GameObject VirtualCamera;
    private GameObject player;

    //Inventory Variables
    public GameObject slotHolder;
    public GameObject shop;
    public TMP_Text prompt;
    public TMP_Text pointDisplay;
    private GameObject[] allObjects;
    private bool [] states;
    private bool [] canvasStates;
    private GameObject camera;
    private GameObject canvas;

    void Start()
    {
        camera = GameObject.Find("Main Camera");
        canvas = GameObject.Find("Canvas");
        player = Instantiate(PlayerPrefab);
        VirtualCamera.GetComponent<CinemachineVirtualCamera>().Follow = player.transform.GetChild(0).transform;
        player.GetComponent<Inventory>().slotHolder = slotHolder;
        player.GetComponent<Inventory>().shop = shop;
        player.GetComponent<Inventory>().prompt = prompt;
        player.GetComponent<Inventory>().pointDisplay = pointDisplay;

        SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    public void sendToCombat(){
        camera.SetActive(false);
        
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
        
        camera.SetActive(true); 
    }

    public GameObject GetPlayer()
    {
        return player;
    }

    [ServerRpc(RequireOwnership=false)]
    public void SpawnPlayerServerRpc(ulong clientId) {
        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
    }
}
