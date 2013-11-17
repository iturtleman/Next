using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class NetworkManager : MonoBehaviour
{

    private const string typeName = "OurAwesomeUniqueGameName";
    private const string gameName = "UniqueRoomName";
    public string connectionIp = "127.0.0.1";
    public int connectionPort = 25001;
    List<NetworkingPlayerContainer> PlayerObjects = new List<NetworkingPlayerContainer>();
    private bool IsPickingIP = false;
    private bool connecting = false;
    private string failedIp = "";
    NetworkConnectionError res;

    /// <summary>
    /// Adds the provided PlayerWrapper to the cleanup list
    /// </summary>
    /// <param name="player"></param>
    internal void AddPlayer(NetworkingPlayerContainer player)
    {
        PlayerObjects.Add(player);
        //This is just for debuging
        foreach (var po in PlayerObjects)
        {
            Debug.Log(string.Format(
                "Player: {0} Original ViewID: {1}",
                po.Player, po.viewID)
            );
        }
    }

    private void StartServer()
    {
        try
        {
            res = Network.InitializeServer(32, connectionPort, !Network.HavePublicAddress());
            if (res != NetworkConnectionError.NoError)
            {
                Debug.LogError(
                    string.Format("Initialization failed with code:{0}", res)
                );
            }
            MasterServer.RegisterHost(typeName, gameName);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    void OnServerInitialized()
    {
        Debug.Log("Server Initializied");
    }

    void OnConnectedToServer()
    {
        Debug.Log("Connected to server");
        ResetConnectionMenu();
    }

    void OnGUI()
    {

        if (Network.peerType == NetworkPeerType.Disconnected)
        {
            if (IsPickingIP)
            {
                if (GUI.Button(new Rect(10, 30, 120, 20), "Back"))
                {
                    ResetConnectionMenu();
                }
            }
            if (connecting)
            {
                GUIStyle gsty = new GUIStyle();
                gsty.fontSize = 50;
                GUI.Label(new Rect((Screen.width / 2) - 250, (Screen.height / 2) - 250, 500, 500), "Connecting");
            }
            else
            {
                GUI.Label(new Rect(10, 10, 300, 20), "Status: Disconnected");
                if (failedIp != "")
                {
                    GUIStyle gsty = new GUIStyle();
                    gsty.normal.textColor = Color.red;
                    GUI.Label(new Rect(10, 70, 120, 20), string.Format("Could not connect to {0} with error:{1}", failedIp, res), gsty);
                }
                if (IsPickingIP)
                {
                    connectionIp = GUI.TextField(new Rect((Screen.width / 2) - 250, 400, 500, 50), connectionIp);


                    if (GUI.Button(new Rect(10, 50, 120, 20), "Connect"))
                    {
                        res = Network.Connect(connectionIp, connectionPort);
                        connecting = true;
                    }
                }
                else
                {
                    if (GUI.Button(new Rect(10, 30, 120, 20), "Client Connect"))
                    {
                        IsPickingIP = true;
                    }
                    if (GUI.Button(new Rect(10, 50, 120, 20), "Initialize Server"))
                    {
                        StartServer();
                    }
                }
            }
        }
        else
        {
            GUI.Label(new Rect(10, 10, 300, 20),
                Network.peerType == NetworkPeerType.Client ?
                    "Status: Connected as Client" :
                        Network.peerType == NetworkPeerType.Server ?
                        "Status: Connected as Server" :
                        "Status: Connected by magic!(WTF)"//this shouldn't happen.
            );
            if (GUI.Button(new Rect(10, 30, 120, 20), "Disconnect"))
            {
                Network.Disconnect(200);
            }
        }
    }

    private void ResetConnectionMenu()
    {
        IsPickingIP = false;
        failedIp = "";
        connecting = false;
    }



    void OnFailedToConnect(NetworkConnectionError error)
    {
        Debug.Log("Could not connect to server: " + error);
        if (IsPickingIP)
        {
            failedIp = connectionIp + ':' + connectionPort;
            connecting = false;
        }
        else
        {
            ResetConnectionMenu();
        }
    }

    void OnPlayerConnected(NetworkPlayer player)
    {
        int playerNumber = Convert.ToInt32(player.ToString());
        Debug.Log(string.Format("Player {0} connected from {1}:{2}", playerNumber, player, player.ipAddress, player.port));
    }

    void OnPlayerDisconnected(NetworkPlayer player)
    {
        //This will find the viewID's associated with the disconnected player
        foreach (var p in PlayerObjects)
        {
            if (p.Player == player.ToString())
            {
                Debug.Log("Removing " + p.viewID);
                Network.RemoveRPCs(p.viewID);
                Network.Destroy(p.viewID);
            }
        }
        Debug.Log("Size of the list " + PlayerObjects.Count);
        //Keeps the PlayerObjects list from having old info in it
        PlayerObjects.RemoveAll(tempList => tempList.Player == player.ToString());
        Debug.Log("New size " + PlayerObjects.Count);
        //Dont think this is removing any RPC's because no player RPC should be buffered but just in case
        Network.RemoveRPCs(player);
        //Delets the player's Character
        Network.DestroyPlayerObjects(player);

    }

    void OnDisconnectedFromServer(NetworkDisconnection info)
    {
        if (Network.isServer)
            Debug.Log("Local server connection disconnected");
        else
        {
            if (info == NetworkDisconnection.LostConnection)
                Debug.Log("Lost connection to the server");
            else
                Debug.Log("Successfully diconnected from the server");
        }
        Application.LoadLevel(Application.loadedLevel);
    }

    void OnNetworkInstantiate(NetworkMessageInfo info)
    {
        Debug.Log("New object instantiated by " + info.sender);
        NetworkView[] networkViews = GetComponents<NetworkView>();
        Debug.Log("New prefab network instantiated with views - ");
        foreach (NetworkView view in networkViews)
        {
            Debug.Log("- " + view.viewID);
        }
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
