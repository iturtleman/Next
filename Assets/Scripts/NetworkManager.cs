using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour
{

    private const string typeName = "OurAwesomeUniqueGameName";
    private const string gameName = "UniqueRoomName";
    public string connectionIP = "127.0.0.1";
    public int connectionPort = 25001;


    private void StartServer()
    {
        Network.InitializeServer(32, connectionPort, !Network.HavePublicAddress());
        MasterServer.RegisterHost(typeName, gameName);
    }

    void OnServerInitialized()
    {
        Debug.Log("Server Initializied");
    }

    void OnGUI()
    {
        if (Network.peerType == NetworkPeerType.Disconnected)
        {
            GUI.Label(new Rect(10, 10, 300, 20), "Status: Disconnected");
            if (GUI.Button(new Rect(10, 30, 120, 20), "Client Connect"))
            {
                Network.Connect(connectionIP, connectionPort);
            }
            if (GUI.Button(new Rect(10, 50, 120, 20), "Initialize Server"))
            {
                StartServer();
            }
        }
        else if (Network.peerType == NetworkPeerType.Client)
        {
            GUI.Label(new Rect(10, 10, 300, 20), "Status: Connected as Client");
            if (GUI.Button(new Rect(10, 30, 120, 20), "Disconnect"))
            {
                Network.Disconnect(200);
            }
        }
        else if (Network.peerType == NetworkPeerType.Server)
        {
            GUI.Label(new Rect(10, 10, 300, 20), "Status: Connected as Server");
            if (GUI.Button(new Rect(10, 30, 120, 20), "Disconnect"))
            {
                Network.Disconnect(200);
            }
        }
    }

    void OnConnectedToServer()
    {
        Debug.Log("Connected to server");
    }

    void OnDisconnectedFromServer(NetworkDisconnection info)
    {
        if (Network.isServer)
            Debug.Log("Local server connection disconnected");
        else
            if (info == NetworkDisconnection.LostConnection)
                Debug.Log("Lost connection to the server");
            else
                Debug.Log("Successfully diconnected from the server");
    }

    void OnFailedToConnect(NetworkConnectionError error)
    {
        Debug.Log("Could not connect to server: " + error);
    }

    private int playerCount = 0;
    void OnPlayerConnected(NetworkPlayer player)
    {
        Debug.Log("Player " + playerCount++ + " connected from " + player.ipAddress + ":" + player.port);
    }

    void OnPlayerDisconnected(NetworkPlayer player)
    {
        Debug.Log("Clean up after player " + player);
        Network.RemoveRPCs(player);
        Network.DestroyPlayerObjects(player);
        playerCount--;
    }

    void OnNetworkInstantiate(NetworkMessageInfo info)
    {
        Debug.Log("New object instantiated by " + info.sender);
    }

    public int currentHealth;
    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        int health = 0;
        if (stream.isWriting)
        {
            health = currentHealth;
            stream.Serialize(ref health);
        }
        else
        {
            stream.Serialize(ref health);
            currentHealth = health;
        }
    }


}
