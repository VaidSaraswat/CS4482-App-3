using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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
    public TMP_Text IPAddressText;

    private Dictionary<ulong, string> m_PlayerNames = new Dictionary<ulong, string>(); 
    private NetworkVariable<int> m_PlayerCount = new NetworkVariable<int>();
    private StringContainer m_IPAddress = new StringContainer();

    void Start()
    {
        if(NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += RemovePlayerName;
            ServerOverlay.SetActive(true);
            SetPlayerNameServerRpc(NetworkManager.Singleton.LocalClientId, PlayerPrefs.GetString("PlayerName", "Host"));

            var host = Dns.GetHostEntry(Dns.GetHostName());
            var ips = host.AddressList.Where(obj => obj.AddressFamily == AddressFamily.InterNetwork).ToArray();
            if(ips.Length == 0)
            {
                throw new System.Exception("No network adapters with an IPv4 address in the system!");
            }
            m_IPAddress.Text = ips[0].ToString();
            IPAddressText.text = "IP Address: " + m_IPAddress.Text;
        }
        else
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += ServerTerminate;
            ClientOverlay.SetActive(true);
            SetPlayerNameServerRpc(NetworkManager.Singleton.LocalClientId, PlayerPrefs.GetString("PlayerName", "Default User"));
            IPAddressText.text = "IP Address: " + m_IPAddress.Text;
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

    public override void OnDestroy()
    {
        if(NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= RemovePlayerName;
        }
        else
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= ServerTerminate;
        }
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
        var playerNames = m_PlayerNames.Select(x => x.Value).ToArray().Aggregate("", (current, next) => current + "\n" + next).Substring(1);
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
        var playerNames = m_PlayerNames.Select(x => x.Value).ToArray().Aggregate("", (current, next) => current + "\n" + next).Substring(1);
        UpdatePlayerNamesClientRpc(playerNames);
    }
}

//Taken from Unity Documentation
//https://docs-multiplayer.unity3d.com/netcode/current/basics/networkvariable/index.html#strings-networkvariable-or-custom-networkvariable
public class StringContainer : NetworkVariableBase
{
    public string Text = default;

    public override void WriteField(FastBufferWriter writer)
    {
        if (string.IsNullOrEmpty(Text))
        {
            writer.WriteValueSafe(0);
            return;
        }

        var textByteArray = System.Text.Encoding.ASCII.GetBytes(Text);

        writer.WriteValueSafe(textByteArray.Length);
        var toalBytesWritten = 0;
        var bytesRemaining = textByteArray.Length;
        while (bytesRemaining > 0)
        {
            writer.WriteValueSafe(textByteArray[toalBytesWritten]);
            toalBytesWritten++;
            bytesRemaining = textByteArray.Length - toalBytesWritten;
        }
    }

    public override void ReadField(FastBufferReader reader)
    {
        Text = string.Empty;
        var stringSize = (int)0;
        reader.ReadValueSafe(out stringSize);

        if (stringSize == 0)
        {
            return;
        }

        var byteArray = new byte[stringSize];
        var tempByte = (byte)0;
        for(int i = 0; i < stringSize; i++)
        {
            reader.ReadValueSafe(out tempByte);
            byteArray[i] = tempByte;
        }
        
        Text = System.Text.Encoding.ASCII.GetString(byteArray);
    }

    public override void ReadDelta(FastBufferReader reader, bool keepDirtyDelta)
    {
    }

    public override void WriteDelta(FastBufferWriter writer)
    {
    }
}