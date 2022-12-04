using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class MovementManager : NetworkBehaviour
{
    //public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();
    public NetworkVariable<ulong> playerId = new NetworkVariable<ulong>();
    public NetworkList<ulong> playerIds;
    private ulong[] playerIdList;

    [Header("Multiplayer Character Input Values")]
    public Vector2 move;
	public Vector2 look;
	public bool jump;
	public bool sprint;

    void Awake()
    {
        playerIds = new NetworkList<ulong>();
        playerIds.OnListChanged += (newList) => { DetermineList(newList); };
    }

    void DetermineList(NetworkListEvent<ulong> changes)
    {
        ulong[] list = new ulong[playerIds.Count];
        for (int i = 0; i < playerIds.Count; i++)
        {
            list[i] = playerIds[i];
        }
        playerIdList = list;
    }

    void Start()
    {
        if(playerId.Value != NetworkManager.Singleton.LocalClientId)
        {
            Destroy(GetComponent<StarterAssets.StarterAssetsInputs>());
            Destroy(GetComponent<StarterAssets.ThirdPersonController>());
            Destroy(GetComponent<PlayerInput>());
            Destroy(GetComponent<Inventory>());
        }
        else 
        {
            GetComponent<PlayerInput>().enabled = true;
            GetComponent<StarterAssets.ThirdPersonController>().enabled = true;
            Destroy(GetComponent<MPPersonController>());
        }
    }

    // void Update()
    // {
    //     UpdatePositionServerRpc(transform.position);
    //     if(playerId.Value != NetworkManager.Singleton.LocalClientId)
    //     {
    //         transform.position = Position.Value;
    //     }
    // }
    
    // [ServerRpc(RequireOwnership=false)]
    // void UpdatePositionServerRpc(Vector3 position)
    // {
    //     Position.Value = position;
    // }

    [ServerRpc(RequireOwnership=false)]
	public void MoveInputServerRpc(Vector2 newMoveDirection)
    {
        MoveInputClientRpc(newMoveDirection, new ClientRpcParams {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = playerIdList
            }
        });
    }

    [ClientRpc]
    //also send camera input
    void MoveInputClientRpc(Vector2 newMoveDirection, ClientRpcParams clientRpcParams)
    {
        move = newMoveDirection;
    }

    [ServerRpc(RequireOwnership=false)]
	public void LookInputServerRpc(Vector2 newLookDirection)
    {
        LookInputClientRpc(newLookDirection, new ClientRpcParams {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = playerIdList
            }
        });
    }

    [ClientRpc]
    void LookInputClientRpc(Vector2 newLookDirection, ClientRpcParams clientRpcParams)
    {
        look = newLookDirection;
    }

    [ServerRpc(RequireOwnership=false)]
	public void JumpInputServerRpc(bool newJumpState)
    {
        JumpInputClientRpc(newJumpState, new ClientRpcParams {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = playerIdList
            }
        });
    }

    [ClientRpc]
    void JumpInputClientRpc(bool newJumpState, ClientRpcParams clientRpcParams)
    {
        jump = newJumpState;
    }

    [ServerRpc(RequireOwnership=false)]
	public void SprintInputServerRpc(bool newSprintState)
    {
        SprintInputClientRpc(newSprintState, new ClientRpcParams {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = playerIdList
            }
        });
    }

    [ClientRpc]
    void SprintInputClientRpc(bool newSprintState, ClientRpcParams clientRpcParams)
    {
        sprint = newSprintState;
    }
}
