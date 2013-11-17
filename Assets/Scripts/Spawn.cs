using UnityEngine;
using System.Collections;
using System;

public class Spawn : MonoBehaviour {

    public NetworkManager netMan;
    public Transform cubePrefabTransform;

    void SpawnPlayer(NetworkPlayer player)
    {
        int playerNumber = Convert.ToInt32(player.ToString());
        Transform newPlayerTransform = (Transform)Network.Instantiate(cubePrefabTransform, transform.position, transform.rotation, 0);
        newPlayerTransform.Translate(new Vector3(0, playerNumber * 10, 0), Space.World);
    }

    void OnServerInitialized()
    {
        Debug.Log("Server Initializied");
        SpawnPlayer(Network.player);
    }

    void OnConnectedToServer()
    {
        Debug.Log("Connected to server");
        SpawnPlayer(Network.player);
    }
}
