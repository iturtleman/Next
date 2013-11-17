using UnityEngine;
using System.Collections;

public class Spawn : MonoBehaviour {

    public Transform cubePrefabTransform;

    void SpawnPlayer()
    {
        Transform newPlayerTransform = (Transform)Network.Instantiate(cubePrefabTransform, transform.position, transform.rotation, 0);
        
    }

    void OnServerInitialized()
    {
        Debug.Log("Server Initializied");
        SpawnPlayer();
    }

    void OnConnectedToServer()
    {
        Debug.Log("Connected to server");
        SpawnPlayer();
    }
}
