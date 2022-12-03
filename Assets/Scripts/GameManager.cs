using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

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

    void Start()
    {
        player = Instantiate(PlayerPrefab);
        VirtualCamera.GetComponent<CinemachineVirtualCamera>().Follow = player.transform.GetChild(0).transform;
        player.GetComponent<Inventory>().slotHolder = slotHolder;
        player.GetComponent<Inventory>().shop = shop;
        player.GetComponent<Inventory>().prompt = prompt;
        player.GetComponent<Inventory>().pointDisplay = pointDisplay;

        SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId);
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
