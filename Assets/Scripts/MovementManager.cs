using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class MovementManager : NetworkBehaviour
{
    public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();
    public NetworkVariable<ulong> playerId;

    void Start()
    {
        if(playerId.Value != NetworkManager.Singleton.LocalClientId)
        {
            Destroy(GetComponent<PlayerInput>());
            Destroy(GetComponent<Inventory>());
        }
    }

    void Update()
    {
        UpdatePositionServerRpc(transform.position);
        if(playerId.Value != NetworkManager.Singleton.LocalClientId)
        {
            transform.position = Position.Value;
        }
    }
    
    [ServerRpc(RequireOwnership=false)]
    void UpdatePositionServerRpc(Vector3 position)
    {
        Position.Value = position;
    }
}
