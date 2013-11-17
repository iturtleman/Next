using UnityEngine;
using System.Collections;
using System;

public class SpawnAuthorative : MonoBehaviour
{
    public NetworkManager netMan;
    public Transform playerPrefab;
    //public ArrayList playerScripts = new ArrayList();

    /// <summary>
    /// Spawns player where we want it and caches the script it runs. 
    /// Then lets everyone clients know owner. 
    /// </summary>
    /// <param name="player"></param>
    void SpawnPlayer(NetworkPlayer player)
    {
        int playerNumber = Convert.ToInt32(player.ToString());
        Debug.Log(string.Format("Spawning player {0} connected from {1}:{2}", playerNumber, player.ipAddress, player.port));
        Transform newPlayer = (Transform)Network.Instantiate(playerPrefab, transform.position, transform.rotation, playerNumber);
        //move objects up a bit
        newPlayer.Translate(new Vector3(0, playerNumber * 0, 0), Space.World);

        //Makes sure that the player is not the server
        if (player.ToString() != "0")
        {
            NetworkView nView;
            nView = newPlayer.GetComponent<NetworkView>();

            //Keep track of the original viewID so that it can be removed from the RPC buffer
            netMan.AddPlayer(new NetworkingPlayerContainer(player.ToString(), nView.viewID));
        }
        
        //let everyone know (includes us)
        NetworkView theNetworkView = newPlayer.networkView;
        theNetworkView.RPC("SetPlayer", RPCMode.AllBuffered, player);
    }

    /// <summary>
    /// When we set up server
    /// </summary>
    void OnServerInitialized()
    {
        Debug.Log(string.Format(
            "Spawning local player as {0}", 
            Network.isClient?"Client":"Server")
        );
        SpawnPlayer(Network.player);
    }

    /// <summary>
    /// When Player connects
    /// </summary>
    /// <param name="player"></param>
    void OnPlayerConnected(NetworkPlayer player)
    {
        Debug.Log(string.Format(
            "Spawning remote player as {0}",
            Network.isClient ? "Client" : "Server")
        );
        SpawnPlayer(player);
    }

    /// <summary>
    /// When we lose connection to a player
    /// </summary>
    /// <param name="player">Player who disconnected</param>
    void OnPlayerDisconnected(NetworkPlayer player)
    {
        int playerNumber = Convert.ToInt32(player.ToString());
        Debug.Log(string.Format("Player {0} disconnected from the game", playerNumber));
    }

}
