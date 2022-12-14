using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using TMPro;

public class MovementManager : NetworkBehaviour
{
    public StringContainer playerName = new StringContainer();
    public NetworkVariable<ulong> playerId = new NetworkVariable<ulong>();
    public NetworkList<ulong> playerIds;
    public int score;
    private ulong[] playerIdList;

    [Header("Multiplayer Character Input Values")]
    public Vector2 move;
	public Vector2 look;
	public bool jump;
	public bool sprint;
    public float cameraAngle;

    private GameObject player;
    public Canvas playerCanvas;
    public TMP_Text playerNameText;

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
            playerCanvas.worldCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
            playerNameText.text = playerName.Text;
        }
        else 
        {
            GetComponent<PlayerInput>().enabled = true;
            GetComponent<StarterAssets.ThirdPersonController>().enabled = true;
            Destroy(GetComponent<MPPersonController>());
            InvokeRepeating("SyncPlayerPosition", 5.0f, 5.0f);
        }
    }

    void Update()
    {
        if(playerId.Value == NetworkManager.Singleton.LocalClientId)
        {
            score = GetComponent<Inventory>().getPoints();
            SetScoreServerRpc(score);
        }
        else
        {
            Quaternion lookRotation = GameObject.Find("Main Camera").GetComponent<Camera>().transform.rotation;
            playerCanvas.transform.rotation = lookRotation;
        }
    }

    public void SyncPlayerPosition()
    {
        SyncPlayerPositionServerRpc(transform.position);
    }

    [ServerRpc(RequireOwnership=false)]
    void SyncPlayerPositionServerRpc(Vector3 newPosition)
    {
        SyncPlayerPositionClientRpc(newPosition, new ClientRpcParams {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = playerIdList
            }
        });
    }

    [ClientRpc]
    void SyncPlayerPositionClientRpc(Vector3 newPosition, ClientRpcParams clientRpcParams)
    {
        GetComponent<CharacterController>().enabled = false;
        transform.position = newPosition;
        GetComponent<CharacterController>().enabled = true;
    }

    [ServerRpc(RequireOwnership=false)]
	void SetScoreServerRpc(int newScore)
    {
        SetScoreClientRpc(newScore, new ClientRpcParams {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = playerIdList
            }
        });
    }

    [ClientRpc]
    void SetScoreClientRpc(int newScore, ClientRpcParams clientRpcParams)
    {
        score = newScore;
    }

    [ServerRpc(RequireOwnership=false)]
	public void CameraAngleServerRpc(float newCameraAngle)
    {
        CameraAngleClientRpc(newCameraAngle, new ClientRpcParams {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = playerIdList
            }
        });
    }

    [ClientRpc]
    void CameraAngleClientRpc(float newCameraAngle, ClientRpcParams clientRpcParams)
    {
        cameraAngle = newCameraAngle;
    }

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

    [ServerRpc(RequireOwnership=false)]
    public void SetMoveSpeedServerRpc(float newMoveSpeed)
    {
        SetMoveSpeedClientRpc(newMoveSpeed, new ClientRpcParams {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = playerIdList
            }
        });
    }

    [ClientRpc]
    void SetMoveSpeedClientRpc(float newMoveSpeed, ClientRpcParams clientRpcParams)
    {
        GetComponent<MPPersonController>().MoveSpeed = newMoveSpeed;
    }
}